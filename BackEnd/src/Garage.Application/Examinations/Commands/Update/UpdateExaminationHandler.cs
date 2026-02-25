using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Clients.Commands.Update;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Domain.InvoiceManagement.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Update;

public sealed class UpdateExaminationHandler(
    IRepository<Examination>  examinationRepo,
    IRepository<Vehicle>      vehicleRepo,
    IRepository<Invoice>      invoiceRepo,
    IReadRepository<Client>   clientRepo,
    IMediator                 mediator,
    IUnitOfWork               unitOfWork,
    ExaminationService        examinationService)
    : BaseCommandHandler<UpdateExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(UpdateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // ── 1. Load examination ───────────────────────────────────────────────
        var examination = await examinationRepo.QueryTracking()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null)
            return Fail("Examination not found.");

        // ── 2. Lookups (before any writes) ────────────────────────────────────
        var lookupsResult = await examinationService.LoadVehicleLookupsAsync(req, ct);
        if (!lookupsResult.Succeeded) return Fail(lookupsResult.Error!);
        var (manufacturer, carMark) = lookupsResult.Value;

        var servicesResult = await examinationService.LoadServicesAsync(req, ct);
        if (!servicesResult.Succeeded) return Fail(servicesResult.Error!);

        // ── 3. Parse enums ────────────────────────────────────────────────────
        var enumsResult = examinationService.ParseVehicleEnums(req);
        if (!enumsResult.Succeeded) return Fail(enumsResult.Error!);
        var (mileageUnit, transmission) = enumsResult.Value;

        // ── 4. Build plate ────────────────────────────────────────────────────
        var plateResult = examinationService.BuildPlate(req);
        if (!plateResult.Succeeded) return Fail(plateResult.Error!);

        // ── 5. All writes inside one transaction ──────────────────────────────
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

            // 5b. Update Vehicle ──────────────────────────────────────────────
            var vehicleId = examination.Vehicle.VehicleId;
            var vehicle   = await vehicleRepo.QueryTracking()
                .FirstOrDefaultAsync(v => v.Id == vehicleId, ct);

            if (vehicle is null)
            {
                await tx.RollbackAsync(ct);
                return Fail("Vehicle not found.");
            }

            vehicle.Update(
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

            // 5c. Update examination ──────────────────────────────────────────
            var clientRef = new ClientReference(
                ClientId:    client.Id,
                NameAr:      client.NameAr,
                NameEn:      client.NameEn,
                PhoneNumber: client.PhoneNumber,
                Email:       req.ClientEmail);

            examination.Update(req.HasWarranty, req.HasPhotos, req.MarketerCode, req.Notes);
            examination.UpdateClientSnapshot(clientRef);
            examination.UpdateVehicleSnapshot(vehicle.ToSnapshot());

            // 5d. Replace items if provided (Draft only) ──────────────────────
            var itemsReplaced = false;
            if (req.Items is { Count: > 0 })
            {
                if (examination.Status != ExaminationStatus.Draft)
                    throw new InvalidOperationException("Items can only be replaced in Draft status.");

                foreach (var item in examination.Items.ToList())
                    examination.RemoveItem(item.Service.ServiceId);

                await examinationService.AddItemsAsync(examination, req, servicesResult.Value, ct);
                itemsReplaced = true;
            }

            await unitOfWork.SaveChangesAsync(ct);

            // 5e. Sync linked invoice (if examination not delivered) ──────────
            if (examination.Status != ExaminationStatus.Delivered)
            {
                var invoice = await invoiceRepo.QueryTracking()
                    .Include(i => i.Items)
                    .FirstOrDefaultAsync(i => i.ExaminationId == examination.Id, ct);

                if (invoice is not null)
                {
                    // Sync client snapshot
                    invoice.UpdateClientSnapshot(clientRef);

                    // Sync items if they were replaced and invoice is still Draft
                    if (itemsReplaced && invoice.Status == InvoiceStatus.Draft)
                    {
                        foreach (var invoiceItem in invoice.Items.ToList())
                            invoice.RemoveItem(invoiceItem.Id);

                        foreach (var examItem in examination.Items)
                        {
                            invoice.AddItem(
                                description:   examItem.Service.NameEn,
                                quantity:      1,
                                unitPrice:     examItem.Price,
                                serviceId:     examItem.Service.ServiceId,
                                serviceNameAr: examItem.Service.NameAr,
                                serviceNameEn: examItem.Service.NameEn);
                        }
                    }

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
