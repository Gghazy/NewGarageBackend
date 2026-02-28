using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Branches.Entities;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Application.Invoices;
using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Create;

public sealed class CreateExaminationHandler(
    IRepository<Examination>  examinationRepo,
    IRepository<Vehicle>      vehicleRepo,
    IReadRepository<Client>   clientRepo,
    IReadRepository<Branch>   branchRepo,
    IUnitOfWork               unitOfWork,
    ExaminationService        examinationService,
    InvoiceSyncService        invoiceSyncService)
    : BaseCommandHandler<CreateExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var isDraft = !req.StartAfterSave;

        // ── 1. Client lookup (always required) ──────────────────────────────

        if (!req.ClientId.HasValue || req.ClientId.Value == Guid.Empty)
            return Fail("Client is required.");

        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == req.ClientId.Value, ct);
        if (client is null) return Fail("Client not found.");

        var clientRef = new ClientReference(
            ClientId:    client.Id,
            NameAr:      client.NameAr,
            NameEn:      client.NameEn,
            PhoneNumber: client.PhoneNumber,
            Email:       req.ClientEmail);

        // ── 2. Branch / Vehicle / Services lookups (skip for draft if empty) ─

        Branch? branch = null;
        if (req.BranchId != Guid.Empty)
        {
            branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
            if (branch is null) return Fail("Branch not found.");
        }
        else if (!isDraft)
            return Fail("Branch is required.");

        var branchRef = branch is not null
            ? new BranchReference(BranchId: branch.Id, NameAr: branch.NameAr, NameEn: branch.NameEn)
            : new BranchReference(Guid.Empty, "", "");

        // Vehicle lookups
        VehicleSnapshot vehicleSnapshot;
        Vehicle? vehicle = null;
        bool hasVehicleData = req.ManufacturerId != Guid.Empty && req.CarMarkId != Guid.Empty;

        if (hasVehicleData)
        {
            var lookupsResult = await examinationService.LoadVehicleLookupsAsync(req, ct);
            if (!lookupsResult.Succeeded) return Fail(lookupsResult.Error!);
            var (manufacturer, carMark) = lookupsResult.Value;

            var enumsResult = examinationService.ParseVehicleEnums(req);
            if (!enumsResult.Succeeded) return Fail(enumsResult.Error!);
            var (mileageUnit, transmission) = enumsResult.Value;

            var plateResult = examinationService.BuildPlate(req);
            if (!plateResult.Succeeded) return Fail(plateResult.Error!);

            vehicle = Vehicle.Create(
                manufacturerId:     req.ManufacturerId,
                manufacturerNameAr: manufacturer.NameAr,
                manufacturerNameEn: manufacturer.NameEn,
                carMarkId:          req.CarMarkId,
                carMarkNameAr:      carMark.NameAr,
                carMarkNameEn:      carMark.NameEn,
                year:               req.Year,
                color:              req.Color,
                vin:                req.Vin,
                hasPlate:           req.HasPlate,
                plate:              plateResult.Value,
                mileage:            req.Mileage,
                mileageUnit:        mileageUnit,
                transmission:       transmission);

            vehicleSnapshot = vehicle.ToSnapshot();
        }
        else if (!isDraft)
        {
            return Fail("Vehicle data is required.");
        }
        else
        {
            vehicleSnapshot = new VehicleSnapshot(
                Guid.Empty, Guid.Empty, "", "", Guid.Empty, "", "",
                null, null, null, false, null, null, MileageUnit.Km, null);
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

        // ── 3. Parse examination type ────────────────────────────────────────

        var examinationType = ExaminationType.Regular;
        if (!string.IsNullOrWhiteSpace(req.Type))
        {
            if (!Enum.TryParse<ExaminationType>(req.Type, ignoreCase: true, out examinationType))
                return Fail($"Invalid examination type '{req.Type}'. Use Regular, Warranty or PrePurchase.");
        }

        // ── 4. One atomic transaction ─────────────────────────────────────────

        await using var tx = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 4a. Vehicle ─────────────────────────────────────────────────────
            if (vehicle is not null)
                await vehicleRepo.AddAsync(vehicle, ct);

            // 4b. Examination ─────────────────────────────────────────────────
            var examination = Examination.Create(
                client:       clientRef,
                branch:       branchRef,
                vehicle:      vehicleSnapshot,
                type:         examinationType,
                hasWarranty:  req.HasWarranty,
                hasPhotos:    false);

            if (!string.IsNullOrWhiteSpace(req.Notes))
                examination.SetNotes(req.Notes);

            // 4c. Items (operational only – no pricing) ────────────────────
            if (hasItems && servicesList is not null)
                ExaminationService.AddItems(examination, req, servicesList);

            // 4c-2. Start if requested ───────────────────────────────────
            if (req.StartAfterSave)
                examination.Start();

            await examinationRepo.AddAsync(examination, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // 4d. Auto-create linked Invoice only when NOT draft ────────
            if (req.StartAfterSave)
            {
                var overridePrices = InvoiceSyncService.BuildOverridePrices(req.Items);
                await invoiceSyncService.CreateInvoiceFromExaminationAsync(
                    examination, clientRef, branchRef, overridePrices, ct);
            }

            await tx.CommitAsync(ct);

            return Ok(examination.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Fail($"Failed to create examination: {ex.Message}");
        }
    }
}
