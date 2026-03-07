using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Application.Examinations.Queries;
using Garage.Contracts.Examinations;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetConsolidated;

public sealed class GetConsolidatedByExaminationQueryHandler(
    IReadRepository<Invoice> invoiceRepo,
    IReadRepository<Examination> examinationRepo)
    : BaseQueryHandler<GetConsolidatedByExaminationQuery, ConsolidatedInvoiceResponse?>
{
    public override async Task<ConsolidatedInvoiceResponse?> Handle(
        GetConsolidatedByExaminationQuery request, CancellationToken ct)
    {
        var invoices = await invoiceRepo.Query()
            .Where(i => i.ExaminationId == request.ExaminationId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(InvoiceProjection.ToDto)
            .ToListAsync(ct);

        if (invoices.Count == 0)
            return null;

        var exam = await examinationRepo.Query()
            .Where(e => e.Id == request.ExaminationId)
            .Select(ExaminationProjection.ToDto)
            .FirstOrDefaultAsync(ct);

        // Compute totalPaid & totalRefunded directly from domain
        var paymentSums = await invoiceRepo.Query()
            .Where(i => i.ExaminationId == request.ExaminationId)
            .SelectMany(i => i.Payments)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalPaid = g.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount),
                TotalRefunded = g.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount),
            })
            .FirstOrDefaultAsync(ct);

        var totalPaid = paymentSums?.TotalPaid ?? 0;
        var totalRefunded = paymentSums?.TotalRefunded ?? 0;

        // Account for standalone refund invoices (no payment records)
        var refundInvoiceTotal = await invoiceRepo.Query()
            .Where(i => i.ExaminationId == request.ExaminationId
                     && i.Type == InvoiceType.Refund)
            .SumAsync(i => i.TotalWithTax.Amount, ct);

        // -- Consolidation logic (sign-based merging) --
        var itemMap = new Dictionary<string, (string Desc, string? NameAr, string? NameEn, decimal UnitPrice, decimal TotalPrice, string Currency)>();
        decimal discountAmount = 0, taxAmount = 0, totalWithTax = 0;

        foreach (var inv in invoices)
        {
            var sign = inv.Type == "Refund" ? -1 : 1;
            discountAmount += sign * inv.DiscountAmount;
            taxAmount      += sign * inv.TaxAmount;
            totalWithTax   += sign * inv.TotalWithTax;

            foreach (var item in inv.Items)
            {
                var key = item.ServiceId?.ToString() ?? item.Description;
                if (itemMap.TryGetValue(key, out var existing))
                {
                    itemMap[key] = existing with
                    {
                        TotalPrice = existing.TotalPrice + sign * item.TotalPrice,
                    };
                }
                else
                {
                    itemMap[key] = (item.Description, item.ServiceNameAr, item.ServiceNameEn,
                        item.UnitPrice, sign * item.TotalPrice, item.Currency);
                }
            }
        }

        var items = itemMap.Values
            .Where(it => Math.Abs(it.TotalPrice) > 0.001m)
            .Select(it => new ConsolidatedItemResponse(
                it.Desc, it.NameAr, it.NameEn, it.UnitPrice, it.TotalPrice, it.Currency))
            .ToList();

        var first = invoices[0];
        var subTotal = items.Sum(it => it.TotalPrice);
        var netPaid = totalPaid - totalRefunded - refundInvoiceTotal;

        return new ConsolidatedInvoiceResponse(
            ClientNameAr:        first.ClientNameAr,
            ClientNameEn:        first.ClientNameEn,
            ClientPhone:         first.ClientPhone,
            BranchNameAr:        first.BranchNameAr,
            BranchNameEn:        first.BranchNameEn,
            ManufacturerNameAr:  exam?.ManufacturerNameAr,
            ManufacturerNameEn:  exam?.ManufacturerNameEn,
            CarMarkNameAr:       exam?.CarMarkNameAr,
            CarMarkNameEn:       exam?.CarMarkNameEn,
            Year:                exam?.Year,
            Color:               exam?.Color,
            Vin:                 exam?.Vin,
            PlateLetters:        exam?.PlateLetters,
            PlateNumbers:        exam?.PlateNumbers,
            Mileage:             exam?.Mileage,
            MileageUnit:         exam?.MileageUnit,
            Items:               items,
            SubTotal:            subTotal,
            DiscountAmount:      discountAmount,
            TaxAmount:           taxAmount,
            TotalWithTax:        totalWithTax,
            TotalPaid:           netPaid,
            Balance:             totalWithTax - netPaid,
            Currency:            first.Currency,
            InvoiceCount:        invoices.Count
        );
    }
}
