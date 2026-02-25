using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Branches.Entities;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Vehicles;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Create;

public sealed class CreateExaminationHandler(
    IRepository<Examination>  examinationRepo,
    IRepository<Vehicle>      vehicleRepo,
    IReadRepository<Client>   clientRepo,
    IReadRepository<Branch>   branchRepo,
    IUnitOfWork               unitOfWork,
    ExaminationService        examinationService)
    : BaseCommandHandler<CreateExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // ── 1. All lookups / validations first (no DB writes yet) ─────────────

        if (!req.ClientId.HasValue || req.ClientId.Value == Guid.Empty)
            return Fail("Client is required.");

        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == req.ClientId.Value, ct);
        if (client is null) return Fail("Client not found.");

        var branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        var lookupsResult = await examinationService.LoadVehicleLookupsAsync(req, ct);
        if (!lookupsResult.Succeeded) return Fail(lookupsResult.Error!);
        var (manufacturer, carMark) = lookupsResult.Value;

        var servicesResult = await examinationService.LoadServicesAsync(req, ct);
        if (!servicesResult.Succeeded) return Fail(servicesResult.Error!);

        // ── 2. Parse enums ────────────────────────────────────────────────────

        var enumsResult = examinationService.ParseVehicleEnums(req);
        if (!enumsResult.Succeeded) return Fail(enumsResult.Error!);
        var (mileageUnit, transmission) = enumsResult.Value;

        if (!Enum.TryParse<ExaminationType>(req.Type, ignoreCase: true, out var examinationType))
            return Fail($"Invalid examination type '{req.Type}'. Use Regular, Warranty or PrePurchase.");

        // ── 3. Build plate ────────────────────────────────────────────────────

        var plateResult = examinationService.BuildPlate(req);
        if (!plateResult.Succeeded) return Fail(plateResult.Error!);

        // ── 4. One atomic transaction ─────────────────────────────────────────

        await using var tx = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 4a. Vehicle ─────────────────────────────────────────────────────
            var vehicle = Vehicle.Create(
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

            await vehicleRepo.AddAsync(vehicle, ct);

            // 4b. Examination ─────────────────────────────────────────────────
            var clientRef = new ClientReference(
                ClientId:    client.Id,
                NameAr:      client.NameAr,
                NameEn:      client.NameEn,
                PhoneNumber: client.PhoneNumber,
                Email:       req.ClientEmail);

            var branchRef = new BranchReference(
                BranchId: branch.Id,
                NameAr:   branch.NameAr,
                NameEn:   branch.NameEn);

            var examination = Examination.Create(
                client:       clientRef,
                branch:       branchRef,
                vehicle:      vehicle.ToSnapshot(),
                type:         examinationType,
                hasWarranty:  req.HasWarranty,
                hasPhotos:    req.HasPhotos,
                marketerCode: req.MarketerCode);

            if (!string.IsNullOrWhiteSpace(req.Notes))
                examination.SetNotes(req.Notes);

            // 4c. Items ───────────────────────────────────────────────────────
            await examinationService.AddItemsAsync(examination, req, servicesResult.Value, ct);

            await examinationRepo.AddAsync(examination, ct);
            await unitOfWork.SaveChangesAsync(ct);
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
