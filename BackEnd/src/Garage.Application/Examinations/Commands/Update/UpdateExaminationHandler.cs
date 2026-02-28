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
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.Services.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Update;

public sealed class UpdateExaminationHandler(
    IRepository<Examination>  examinationRepo,
    IRepository<Vehicle>      vehicleRepo,
    IReadRepository<Client>   clientRepo,
    IReadRepository<Branch>   branchRepo,
    IMediator                 mediator,
    IUnitOfWork               unitOfWork,
    ExaminationService        examinationService,
    InvoiceSyncService        invoiceSyncService)
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

            examination.Update(req.HasWarranty, false, req.Notes);
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
                var invoiceBranchRef = branch is not null
                    ? new BranchReference(branch.Id, branch.NameAr, branch.NameEn)
                    : examination.Branch;

                var overridePrices = InvoiceSyncService.BuildOverridePrices(req.Items);

                await invoiceSyncService.SyncLinkedInvoiceAsync(
                    examination, clientRef, invoiceBranchRef, overridePrices, itemsReplaced, ct);
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
