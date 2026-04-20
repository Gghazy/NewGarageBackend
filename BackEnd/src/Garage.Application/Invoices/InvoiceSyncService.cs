using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Examinations;
using Garage.Contracts.Examinations;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Garage.Domain.PaymentMethods.Entity;
using Garage.Domain.ServiceDiscounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices;

public sealed class InvoiceSyncService(
    IRepository<Invoice>   invoiceRepo,
    IUnitOfWork            unitOfWork,
    ExaminationService     examinationService,
    InvoiceNumberGenerator invoiceNumberGenerator,
    ILookupRepository<PaymentMethodLookup> methodRepo,
    IReadRepository<ServiceDiscount> serviceDiscountRepo)
{
    /// <summary>Build override prices dict from request items.</summary>
    public static Dictionary<Guid, decimal> BuildOverridePrices(List<ExaminationItemRequest>? items)
        => (items ?? [])
            .Where(i => i.OverridePrice.HasValue)
            .ToDictionary(i => i.ServiceId, i => i.OverridePrice!.Value);

    /// <summary>Resolve price: override → service-price → zero.</summary>
    public static Money ResolveUnitPrice(
        Guid serviceId,
        Dictionary<Guid, decimal>? overridePrices,
        Dictionary<Guid, decimal> priceMap)
    {
        if (overridePrices is not null && overridePrices.TryGetValue(serviceId, out var op))
            return Money.Create(op);

        return priceMap.TryGetValue(serviceId, out var p)
            ? Money.Create(p)
            : Money.Zero();
    }

    /// <summary>Add invoice items from examination items.</summary>
    public static void PopulateInvoiceItems(
        Invoice invoice,
        IReadOnlyCollection<ExaminationItem> examItems,
        Dictionary<Guid, decimal>? overridePrices,
        Dictionary<Guid, decimal> priceMap,
        Dictionary<Guid, decimal>? discountMap = null)
    {
        foreach (var examItem in examItems)
        {
            var unitPrice = ResolveUnitPrice(examItem.Service.ServiceId, overridePrices, priceMap);
            var lineTotal = Money.Create(unitPrice.Amount * examItem.Quantity, unitPrice.Currency);

            var discountPercent = 0m;
            if (discountMap is not null && discountMap.TryGetValue(examItem.Service.ServiceId, out var dp))
                discountPercent = dp;

            invoice.AddItem(
                description:     examItem.Service.NameEn,
                unitPrice:       lineTotal,
                serviceId:       examItem.Service.ServiceId,
                serviceNameAr:   examItem.Service.NameAr,
                serviceNameEn:   examItem.Service.NameEn,
                discountPercent: discountPercent);
        }
    }

    /// <summary>
    /// Create a new invoice from an examination.
    /// Used by Create, Update (no-invoice path), and CreateFromExamination handlers.
    /// </summary>
    public async Task<Invoice> CreateInvoiceFromExaminationAsync(
        Examination exam,
        ClientReference client,
        BranchReference branch,
        Dictionary<Guid, decimal>? overridePrices,
        CancellationToken ct)
    {
        var invoice = Invoice.Create(
            client:        client,
            branch:        branch,
            currency:      "SAR",
            examinationId: exam.Id);

        if (!string.IsNullOrWhiteSpace(exam.Notes))
            invoice.SetNotes(exam.Notes);

        var priceMap = await examinationService.LookupServicePricesAsync(
            exam.Items.Select(i => i.Service.ServiceId),
            exam.Vehicle.CarMarkId,
            exam.Vehicle.Year,
            ct);

        var discountMap = await LookupActiveDiscountsAsync(
            exam.Items.Select(i => i.Service.ServiceId), ct);

        PopulateInvoiceItems(invoice, exam.Items, overridePrices, priceMap, discountMap);

        var invNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Invoice, ct);
        invoice.SetInvoiceNumber(invNumber);

        await invoiceRepo.AddAsync(invoice, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return invoice;
    }

    /// <summary>
    /// Sync the linked invoice after examination items change.
    /// Handles 3 strategies: no invoice → create, Issued → replace items, Paid → refund/adjustment.
    /// </summary>
    public async Task SyncLinkedInvoiceAsync(
        Examination exam,
        ClientReference client,
        BranchReference branch,
        Dictionary<Guid, decimal>? overridePrices,
        bool itemsReplaced,
        CancellationToken ct)
    {
        var invoice = await invoiceRepo.QueryTracking()
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.ExaminationId == exam.Id
                                   && i.Type == InvoiceType.Invoice, ct);

        if (invoice is null)
        {
            // No invoice yet (was Draft before) → create one
            await CreateInvoiceFromExaminationAsync(exam, client, branch, overridePrices, ct);
            return;
        }

        if (!itemsReplaced)
        {
            // Items not replaced, just update client snapshot
            invoice.UpdateClientSnapshot(client);
            await unitOfWork.SaveChangesAsync(ct);
            return;
        }

        // Items changed → sync based on invoice status
        var priceMap = await examinationService.LookupServicePricesAsync(
            exam.Items.Select(i => i.Service.ServiceId),
            exam.Vehicle.CarMarkId,
            exam.Vehicle.Year,
            ct);

        var discountMap = await LookupActiveDiscountsAsync(
            exam.Items.Select(i => i.Service.ServiceId), ct);

        if (invoice.Status == InvoiceStatus.Issued)
        {
            await SyncIssuedInvoice(invoice, exam, client, overridePrices, priceMap, ct);
        }
        else if (invoice.Status == InvoiceStatus.Paid)
        {
            await SyncPaidInvoice(invoice, exam, overridePrices, priceMap, discountMap, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);
    }

    // ── Private helpers ─────────────────────────────────────────────────────

    /// <summary>
    /// Looks up active service discounts for the given services.
    /// Returns a map of ServiceId → DiscountPercent.
    /// </summary>
    private async Task<Dictionary<Guid, decimal>?> LookupActiveDiscountsAsync(
        IEnumerable<Guid> serviceIds, CancellationToken ct)
    {
        var ids = serviceIds.Distinct().ToList();
        if (ids.Count == 0) return null;

        var now = DateTime.UtcNow;

        var activeDiscounts = await serviceDiscountRepo.Query()
            .Where(d => ids.Contains(d.ServiceId)
                     && d.IsActive
                     && d.StartDate <= now
                     && d.EndDate >= now)
            .ToListAsync(ct);

        if (activeDiscounts.Count == 0) return null;

        return activeDiscounts.ToDictionary(d => d.ServiceId, d => d.DiscountPercent);
    }

    private async Task SyncIssuedInvoice(
        Invoice invoice,
        Examination exam,
        ClientReference client,
        Dictionary<Guid, decimal>? overridePrices,
        Dictionary<Guid, decimal> priceMap,
        CancellationToken ct)
    {
        invoice.UpdateClientSnapshot(client);

        foreach (var invoiceItem in invoice.Items.ToList())
            invoice.RemoveItem(invoiceItem.Id);

        var discountMap = await LookupActiveDiscountsAsync(
            exam.Items.Select(i => i.Service.ServiceId), ct);

        PopulateInvoiceItems(invoice, exam.Items, overridePrices, priceMap, discountMap);

        await unitOfWork.SaveChangesAsync(ct);
    }

    private async Task SyncPaidInvoice(
        Invoice invoice,
        Examination exam,
        Dictionary<Guid, decimal>? overridePrices,
        Dictionary<Guid, decimal> priceMap,
        Dictionary<Guid, decimal>? discountMap,
        CancellationToken ct)
    {
        var oldItemsByService = invoice.Items
            .Where(i => i.ServiceId.HasValue)
            .ToDictionary(i => i.ServiceId!.Value);

        var newServiceIds = exam.Items
            .Select(i => i.Service.ServiceId)
            .ToHashSet();

        var adjustmentItems = new List<(string desc, Money price, Guid serviceId, string nameAr, string nameEn, decimal adjustmentAmount, decimal discountPercent)>();
        var refundItems = new List<(string desc, Money price, Guid? serviceId, string? nameAr, string? nameEn, decimal adjustmentAmount, decimal discountPercent)>();

        // 1. Added or changed services
        foreach (var examItem in exam.Items)
        {
            var svcId = examItem.Service.ServiceId;
            var newUnitPrice = ResolveUnitPrice(svcId, overridePrices, priceMap);
            var newGross = newUnitPrice.Amount * examItem.Quantity;

            // Apply discount to get the net total for comparison
            var discPct = discountMap is not null && discountMap.TryGetValue(svcId, out var dp) ? dp : 0m;
            var discAmt = discPct > 0 ? Math.Round(newGross * discPct / 100m, 2) : 0m;
            var newNet = newGross - discAmt;

            if (!oldItemsByService.ContainsKey(svcId))
            {
                // New service → full net amount is increase
                var lineTotal = Money.Create(newGross, newUnitPrice.Currency);
                adjustmentItems.Add((examItem.Service.NameEn, lineTotal, svcId, examItem.Service.NameAr, examItem.Service.NameEn, newNet, discPct));
            }
            else
            {
                var oldItem = oldItemsByService[svcId];
                var diff = newNet - oldItem.TotalPrice.Amount;

                if (diff > 0.001m)
                {
                    var rounded = Math.Round(diff, 2);
                    var newPrice = Money.Create(newGross, newUnitPrice.Currency);
                    adjustmentItems.Add((examItem.Service.NameEn, newPrice, svcId, examItem.Service.NameAr, examItem.Service.NameEn, rounded, discPct));
                }
                else if (diff < -0.001m)
                {
                    var rounded = Math.Round(Math.Abs(diff), 2);
                    var newPrice = Money.Create(newGross, newUnitPrice.Currency);
                    refundItems.Add((examItem.Service.NameEn, newPrice, svcId, examItem.Service.NameAr, examItem.Service.NameEn, rounded, discPct));
                }
            }
        }

        // 2. Removed services
        foreach (var oldItem in invoice.Items)
        {
            if (oldItem.ServiceId.HasValue && !newServiceIds.Contains(oldItem.ServiceId.Value))
            {
                refundItems.Add((oldItem.Description, oldItem.TotalPrice, oldItem.ServiceId, oldItem.ServiceNameAr, oldItem.ServiceNameEn, oldItem.TotalPrice.Amount, oldItem.DiscountPercent));
            }
        }

        // 3. Create Adjustment invoice if there's an increase
        if (adjustmentItems.Count > 0)
        {
            var adjustment = Invoice.CreateAdjustment(invoice);
            foreach (var item in adjustmentItems)
                adjustment.AddItem(item.desc, item.price, item.serviceId, item.nameAr, item.nameEn, item.adjustmentAmount, item.discountPercent);

            var adjNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Adjustment, ct);
            adjustment.SetInvoiceNumber(adjNumber);
            await invoiceRepo.AddAsync(adjustment, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }

        // 4. Create Refund invoice if there's a decrease (negative amounts)
        if (refundItems.Count > 0)
        {
            var refundInvoice = Invoice.CreateEmptyRefundInvoice(invoice);
            foreach (var item in refundItems)
                refundInvoice.AddItem(item.desc, item.price, item.serviceId, item.nameAr, item.nameEn, item.adjustmentAmount, item.discountPercent);

            refundInvoice.MarkAsRefunded();

            var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
            refundInvoice.SetInvoiceNumber(refNumber);

            await invoiceRepo.AddAsync(refundInvoice, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
    }
}
