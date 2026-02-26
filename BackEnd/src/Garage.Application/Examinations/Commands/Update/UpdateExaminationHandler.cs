using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Clients.Commands.Update;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Invoices;
using Garage.Contracts.Examinations;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Domain.InvoiceManagement.Invoices;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.Services.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Update;

public sealed class UpdateExaminationHandler(
    IRepository<Examination>  examinationRepo,
    IRepository<Vehicle>      vehicleRepo,
    IRepository<Invoice>      invoiceRepo,
    IReadRepository<Client>   clientRepo,
    IReadRepository<Branch>   branchRepo,
    IMediator                 mediator,
    IUnitOfWork               unitOfWork,
    ExaminationService        examinationService,
    InvoiceNumberGenerator    invoiceNumberGenerator)
    : BaseCommandHandler<UpdateExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(UpdateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var isDraft = !req.StartAfterSave;

        // ── 1. Load examination ───────────────────────────────────────────────
        var examination = await examinationRepo.QueryTracking()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null)
            return Fail("Examination not found.");

        // ── 2. Conditional lookups (skip for draft when data is empty) ────────

        // Branch
        Branch? branch = null;
        if (req.BranchId != Guid.Empty)
        {
            branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
            if (branch is null) return Fail("Branch not found.");
        }
        else if (!isDraft)
            return Fail("Branch is required.");

        // Vehicle lookups
        bool hasVehicleData = req.ManufacturerId != Guid.Empty && req.CarMarkId != Guid.Empty;
        MileageUnit mileageUnit = MileageUnit.Km;
        TransmissionType? transmission = null;
        PlateNumber? plate = null;
        Manufacturer? manufacturer = null;
        CarMark? carMark = null;

        if (hasVehicleData)
        {
            var lookupsResult = await examinationService.LoadVehicleLookupsAsync(req, ct);
            if (!lookupsResult.Succeeded) return Fail(lookupsResult.Error!);
            (manufacturer, carMark) = lookupsResult.Value;

            var enumsResult = examinationService.ParseVehicleEnums(req);
            if (!enumsResult.Succeeded) return Fail(enumsResult.Error!);
            (mileageUnit, transmission) = enumsResult.Value;

            var plateResult = examinationService.BuildPlate(req);
            if (!plateResult.Succeeded) return Fail(plateResult.Error!);
            plate = plateResult.Value;
        }
        else if (!isDraft)
        {
            return Fail("Vehicle data is required.");
        }

        // Services
        List<Service>? servicesList = null;
        bool hasItems = req.Items is { Count: > 0 };
        if (hasItems)
        {
            var servicesResult = await examinationService.LoadServicesAsync(req, ct);
            if (!servicesResult.Succeeded) return Fail(servicesResult.Error!);
            servicesList = servicesResult.Value;
        }

        // ── 3. All writes inside one transaction ──────────────────────────────
        await using var tx = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 5a. Update client ───────────────────────────────────────────────
            var clientId = examination.Client.ClientId;
            var updateClientResult = await mediator.Send(
                new UpdateClientCommand(clientId, ExaminationService.MapToClientRequest(req)), ct);

            if (!updateClientResult.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return Fail(updateClientResult.Error!);
            }

            var client = await clientRepo.QueryTracking()
                .FirstOrDefaultAsync(c => c.Id == clientId, ct);

            if (client is null)
            {
                await tx.RollbackAsync(ct);
                return Fail("Client not found after update.");
            }

            // 5b. Update / Create Vehicle ────────────────────────────────────
            VehicleSnapshot vehicleSnapshot;
            if (hasVehicleData)
            {
                var existingVehicleId = examination.Vehicle.VehicleId;

                if (existingVehicleId != Guid.Empty)
                {
                    // Vehicle exists → update it
                    var vehicle = await vehicleRepo.QueryTracking()
                        .FirstOrDefaultAsync(v => v.Id == existingVehicleId, ct);

                    if (vehicle is null)
                    {
                        await tx.RollbackAsync(ct);
                        return Fail("Vehicle not found.");
                    }

                    vehicle.Update(
                        manufacturerId:     req.ManufacturerId,
                        manufacturerNameAr: manufacturer!.NameAr,
                        manufacturerNameEn: manufacturer.NameEn,
                        carMarkId:          req.CarMarkId,
                        carMarkNameAr:      carMark!.NameAr,
                        carMarkNameEn:      carMark.NameEn,
                        year:               req.Year,
                        color:              req.Color,
                        vin:                req.Vin,
                        hasPlate:           req.HasPlate,
                        plate:              plate,
                        mileage:            req.Mileage,
                        mileageUnit:        mileageUnit,
                        transmission:       transmission);

                    vehicleSnapshot = vehicle.ToSnapshot();
                }
                else
                {
                    // No vehicle yet (was draft) → create new one
                    var vehicle = Vehicle.Create(
                        manufacturerId:     req.ManufacturerId,
                        manufacturerNameAr: manufacturer!.NameAr,
                        manufacturerNameEn: manufacturer.NameEn,
                        carMarkId:          req.CarMarkId,
                        carMarkNameAr:      carMark!.NameAr,
                        carMarkNameEn:      carMark.NameEn,
                        year:               req.Year,
                        color:              req.Color,
                        vin:                req.Vin,
                        hasPlate:           req.HasPlate,
                        plate:              plate,
                        mileage:            req.Mileage,
                        mileageUnit:        mileageUnit,
                        transmission:       transmission);

                    await vehicleRepo.AddAsync(vehicle, ct);
                    vehicleSnapshot = vehicle.ToSnapshot();
                }
            }
            else
            {
                // No vehicle data (still draft) → keep existing snapshot
                vehicleSnapshot = examination.Vehicle;
            }

            // 5c. Update examination ──────────────────────────────────────────
            var clientRef = new ClientReference(
                ClientId:    client.Id,
                NameAr:      client.NameAr,
                NameEn:      client.NameEn,
                PhoneNumber: client.PhoneNumber,
                Email:       req.ClientEmail);

            examination.Update(req.HasWarranty, false, req.MarketerCode, req.Notes);
            examination.UpdateClientSnapshot(clientRef);
            examination.UpdateVehicleSnapshot(vehicleSnapshot);

            // Update branch if provided
            if (branch is not null)
            {
                var branchRef = new BranchReference(
                    BranchId: branch.Id,
                    NameAr:   branch.NameAr,
                    NameEn:   branch.NameEn);
                examination.UpdateBranchSnapshot(branchRef);
            }

            // 5d. Replace items if provided ────────────────────────────────────
            var itemsReplaced = false;
            if (hasItems && servicesList is not null)
            {
                foreach (var item in examination.Items.ToList())
                    examination.RemoveItem(item.Service.ServiceId);

                ExaminationService.AddItems(examination, req, servicesList);
                itemsReplaced = true;
            }

            // 5d-2. Start if requested ───────────────────────────────────
            if (req.StartAfterSave && examination.Status == ExaminationStatus.Draft)
                examination.Start();

            await unitOfWork.SaveChangesAsync(ct);

            // 5e. Sync linked invoice (skip entirely when still Draft) ────────
            if (examination.Status != ExaminationStatus.Draft && examination.Status != ExaminationStatus.Delivered)
            {
                var invoice = await invoiceRepo.QueryTracking()
                    .Include(i => i.Items)
                    .Include(i => i.Payments)
                    .FirstOrDefaultAsync(i => i.ExaminationId == examination.Id
                                           && i.Type == InvoiceType.Invoice, ct);

                var overridePrices = (req.Items ?? new())
                    .Where(i => i.OverridePrice.HasValue)
                    .ToDictionary(i => i.ServiceId, i => i.OverridePrice!.Value);

                if (invoice is null)
                {
                    // ── No invoice yet (was Draft before) → create one ──────────
                    var invoiceBranchRef = branch is not null
                        ? new BranchReference(branch.Id, branch.NameAr, branch.NameEn)
                        : new BranchReference(examination.Branch.BranchId, examination.Branch.NameAr, examination.Branch.NameEn);

                    invoice = Invoice.Create(
                        client:        clientRef,
                        branch:        invoiceBranchRef,
                        currency:      "EGP",
                        examinationId: examination.Id);

                    var priceMap = await examinationService.LookupServicePricesAsync(
                        examination.Items.Select(i => i.Service.ServiceId),
                        req.CarMarkId,
                        req.Year,
                        ct);

                    foreach (var examItem in examination.Items)
                    {
                        var unitPrice = overridePrices.TryGetValue(examItem.Service.ServiceId, out var op)
                            ? Money.Create(op)
                            : priceMap.TryGetValue(examItem.Service.ServiceId, out var p)
                                ? Money.Create(p) : Money.Zero();

                        invoice.AddItem(
                            description:   examItem.Service.NameEn,
                            quantity:      examItem.Quantity,
                            unitPrice:     unitPrice,
                            serviceId:     examItem.Service.ServiceId,
                            serviceNameAr: examItem.Service.NameAr,
                            serviceNameEn: examItem.Service.NameEn);
                    }

                    // Auto-issue
                    var invNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Invoice, ct);
                    invoice.SetInvoiceNumber(invNumber);
                    invoice.Issue();

                    await invoiceRepo.AddAsync(invoice, ct);
                    await unitOfWork.SaveChangesAsync(ct);
                }
                else if (itemsReplaced)
                {
                    // ── Invoice exists & items changed → sync ───────────────────
                    invoice.UpdateClientSnapshot(clientRef);

                    var priceMap = await examinationService.LookupServicePricesAsync(
                        examination.Items.Select(i => i.Service.ServiceId),
                        req.CarMarkId,
                        req.Year,
                        ct);

                    if (invoice.Status == InvoiceStatus.Draft)
                    {
                        // Draft → replace items directly
                        foreach (var invoiceItem in invoice.Items.ToList())
                            invoice.RemoveItem(invoiceItem.Id);

                        foreach (var examItem in examination.Items)
                        {
                            var unitPrice = overridePrices.TryGetValue(examItem.Service.ServiceId, out var op)
                                ? Money.Create(op)
                                : priceMap.TryGetValue(examItem.Service.ServiceId, out var p)
                                    ? Money.Create(p) : Money.Zero();

                            invoice.AddItem(
                                description:   examItem.Service.NameEn,
                                quantity:      examItem.Quantity,
                                unitPrice:     unitPrice,
                                serviceId:     examItem.Service.ServiceId,
                                serviceNameAr: examItem.Service.NameAr,
                                serviceNameEn: examItem.Service.NameEn);
                        }
                    }
                    else if (invoice.Status == InvoiceStatus.Issued
                          || invoice.Status == InvoiceStatus.Paid
                          || invoice.Status == InvoiceStatus.PartiallyPaid)
                    {
                        // Issued/Paid/PartiallyPaid → compare items, create Refund or Debit Note
                        var oldServiceIds = invoice.Items
                            .Where(i => i.ServiceId.HasValue)
                            .Select(i => i.ServiceId!.Value)
                            .ToHashSet();

                        var newServiceIds = examination.Items
                            .Select(i => i.Service.ServiceId)
                            .ToHashSet();

                        // Added services = in exam but not in original invoice
                        var addedItems = examination.Items
                            .Where(i => !oldServiceIds.Contains(i.Service.ServiceId))
                            .ToList();

                        // Removed services = in original invoice but not in exam
                        var removedItems = invoice.Items
                            .Where(i => i.ServiceId.HasValue && !newServiceIds.Contains(i.ServiceId.Value))
                            .ToList();

                        if (addedItems.Count > 0)
                        {
                            // Service added → Debit Note (Adjustment) with actual service items
                            var adjustment = Invoice.CreateAdjustment(invoice);

                            foreach (var examItem in addedItems)
                            {
                                var unitPrice = overridePrices.TryGetValue(examItem.Service.ServiceId, out var op)
                                    ? Money.Create(op)
                                    : priceMap.TryGetValue(examItem.Service.ServiceId, out var p)
                                        ? Money.Create(p) : Money.Zero();

                                adjustment.AddItem(
                                    description:   examItem.Service.NameEn,
                                    quantity:      examItem.Quantity,
                                    unitPrice:     unitPrice,
                                    serviceId:     examItem.Service.ServiceId,
                                    serviceNameAr: examItem.Service.NameAr,
                                    serviceNameEn: examItem.Service.NameEn);
                            }

                            var adjNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Adjustment, ct);
                            adjustment.SetInvoiceNumber(adjNumber);
                            adjustment.Issue();

                            await invoiceRepo.AddAsync(adjustment, ct);
                        }

                        if (removedItems.Count > 0)
                        {
                            // Service removed → Refund Invoice with negative-priced service items
                            var refundInvoice = Invoice.CreateEmptyRefundInvoice(invoice);

                            foreach (var removedItem in removedItems)
                            {
                                refundInvoice.AddItem(
                                    description:   removedItem.Description,
                                    quantity:      removedItem.Quantity,
                                    unitPrice:     removedItem.UnitPrice.Negate(),
                                    serviceId:     removedItem.ServiceId,
                                    serviceNameAr: removedItem.ServiceNameAr,
                                    serviceNameEn: removedItem.ServiceNameEn);
                            }

                            var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
                            refundInvoice.SetInvoiceNumber(refNumber);
                            refundInvoice.Issue();

                            // Add refund transaction on original only if it has payments
                            if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.PartiallyPaid)
                            {
                                var absTotal = Math.Abs(refundInvoice.TotalWithTax.Amount);
                                var refundAmount = Money.Create(absTotal, refundInvoice.TotalWithTax.Currency);
                                invoice.AddRefund(refundAmount, PaymentMethod.Cash, "Auto-refund due to service removal");
                            }

                            await invoiceRepo.AddAsync(refundInvoice, ct);
                        }
                    }

                    await unitOfWork.SaveChangesAsync(ct);
                }
                else
                {
                    // ── Items not replaced, just update client snapshot ──────────
                    invoice.UpdateClientSnapshot(clientRef);
                    await unitOfWork.SaveChangesAsync(ct);
                }
            }

            await tx.CommitAsync(ct);

            return Ok(examination.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Fail($"Failed to update examination: {ex.Message}");
        }
    }
}
