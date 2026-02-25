using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Clients.Commands.Create;
using Garage.Application.Clients.Commands.Update;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Clients;
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

namespace Garage.Application.Examinations.Commands.Create;

public sealed class CreateExaminationHandler : BaseCommandHandler<CreateExaminationCommand, Guid>
{
    private readonly IRepository<Examination>      _examinationRepo;
    private readonly IRepository<Vehicle>          _vehicleRepo;
    private readonly IReadRepository<Client>       _clientRepo;
    private readonly IReadRepository<Branch>       _branchRepo;
    private readonly IReadRepository<Service>      _serviceRepo;
    private readonly IReadRepository<Manufacturer> _manufacturerRepo;
    private readonly IReadRepository<CarMark>      _carMarkRepo;
    private readonly IMediator                     _mediator;
    private readonly IUnitOfWork                   _unitOfWork;

    public CreateExaminationHandler(
        IRepository<Examination>      examinationRepo,
        IRepository<Vehicle>          vehicleRepo,
        IReadRepository<Client>       clientRepo,
        IReadRepository<Branch>       branchRepo,
        IReadRepository<Service>      serviceRepo,
        IReadRepository<Manufacturer> manufacturerRepo,
        IReadRepository<CarMark>      carMarkRepo,
        IMediator                     mediator,
        IUnitOfWork                   unitOfWork)
    {
        _examinationRepo  = examinationRepo;
        _vehicleRepo      = vehicleRepo;
        _clientRepo       = clientRepo;
        _branchRepo       = branchRepo;
        _serviceRepo      = serviceRepo;
        _manufacturerRepo = manufacturerRepo;
        _carMarkRepo      = carMarkRepo;
        _mediator         = mediator;
        _unitOfWork       = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CreateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // ── 1. All lookups / validations first (no DB writes yet) ─────────────

        var branch = await _branchRepo.GetByIdAsync(req.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        var manufacturer = await _manufacturerRepo.GetByIdAsync(req.ManufacturerId, ct);
        if (manufacturer is null) return Fail("Manufacturer not found.");

        var carMark = await _carMarkRepo.GetByIdAsync(req.CarMarkId, ct);
        if (carMark is null) return Fail("Car mark not found.");

        List<Service> services = [];
        if (req.Items != null && req.Items.Count > 0)
        {
            var serviceIds = req.Items.Select(i => i.ServiceId).Distinct().ToList();
            services = await _serviceRepo.Query()
                .Where(s => serviceIds.Contains(s.Id))
                .ToListAsync(ct);

            if (services.Count != serviceIds.Count)
                return Fail("One or more services were not found.");
        }

        // ── 2. Parse enums ────────────────────────────────────────────────────

        if (!Enum.TryParse<MileageUnit>(req.MileageUnit, ignoreCase: true, out var mileageUnit))
            return Fail($"Invalid mileage unit '{req.MileageUnit}'. Use Km or Mile.");

        TransmissionType? transmission = null;
        if (!string.IsNullOrWhiteSpace(req.Transmission))
        {
            if (!Enum.TryParse<TransmissionType>(req.Transmission, ignoreCase: true, out var tp))
                return Fail($"Invalid transmission type '{req.Transmission}'.");
            transmission = tp;
        }

        if (!Enum.TryParse<ExaminationType>(req.Type, ignoreCase: true, out var examinationType))
            return Fail($"Invalid examination type '{req.Type}'. Use Regular, Warranty or PrePurchase.");

        // ── 3. Build plate ────────────────────────────────────────────────────

        PlateNumber? plate = null;
        if (req.HasPlate)
        {
            if (string.IsNullOrWhiteSpace(req.PlateLetters) || string.IsNullOrWhiteSpace(req.PlateNumbers))
                return Fail("Plate letters and numbers are required when vehicle has a plate.");
            plate = PlateNumber.Create(req.PlateLetters, req.PlateNumbers);
        }

        // ── 4. One atomic transaction ─────────────────────────────────────────

        await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 4a. Upsert client via existing Create/Update commands ─────────────
            var clientResult = await UpsertClientAsync(req, ct);
            if (!clientResult.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return Fail(clientResult.Error!);
            }
            var client = clientResult.Value!;

            // 4b. Vehicle ──────────────────────────────────────────────────────
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
                plate:              plate,
                mileage:            req.Mileage,
                mileageUnit:        mileageUnit,
                transmission:       transmission);

            await _vehicleRepo.AddAsync(vehicle, ct);

            // 4c. Examination ──────────────────────────────────────────────────
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

            // 4d. Items ────────────────────────────────────────────────────────
            if (req.Items != null && req.Items.Count > 0)
            {
                var serviceMap = services.ToDictionary(s => s.Id);
                foreach (var itemReq in req.Items)
                {
                    var svc      = serviceMap[itemReq.ServiceId];
                    var snapshot = new ServiceSnapshot(svc.Id, svc.NameAr, svc.NameEn, Money.Zero());
                    var price    = itemReq.OverridePrice.HasValue ? Money.Create(itemReq.OverridePrice.Value) : null;
                    examination.AddItem(snapshot, price);
                }
            }

            await _examinationRepo.AddAsync(examination, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Ok(examination.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Fail($"Failed to create examination: {ex.Message}");
        }
    }

    // ── Client upsert via existing MediatR commands ───────────────────────────

    private async Task<Result<Client>> UpsertClientAsync(CreateExaminationRequest req, CancellationToken ct)
    {
        var clientRequest = MapToClientRequest(req);

        if (req.ClientId.HasValue && req.ClientId.Value != Guid.Empty)
        {
            var existing = await _clientRepo.QueryTracking()
                .FirstOrDefaultAsync(c => c.Id == req.ClientId.Value, ct);

            if (existing is not null)
            {
                var updateResult = await _mediator.Send(
                    new UpdateClientCommand(req.ClientId.Value, clientRequest), ct);

                if (!updateResult.Succeeded)
                    return Result<Client>.Fail(updateResult.Error!);

                var updated = await _clientRepo.QueryTracking()
                    .FirstOrDefaultAsync(c => c.Id == req.ClientId.Value, ct);

                return Result<Client>.Ok(updated!);
            }
        }

        // Create new client
        var createResult = await _mediator.Send(new CreateClientCommand(clientRequest), ct);
        if (!createResult.Succeeded)
            return Result<Client>.Fail(createResult.Error!);

        var created = await _clientRepo.QueryTracking()
            .FirstOrDefaultAsync(c => c.Id == createResult.Value, ct);

        return Result<Client>.Ok(created!);
    }


    private static CreateClientRequest MapToClientRequest(CreateExaminationRequest req) => new(
        Email:                req.ClientEmail ?? string.Empty,
        Type:                 req.ClientType,
        NameAr:               req.ClientNameAr,
        NameEn:               req.ClientNameEn,
        PhoneNumber:          req.ClientPhone,
        CommercialRegister:   req.CommercialRegister,
        TaxNumber:            req.TaxNumber,
        ResourceId:           req.ClientResourceId,
        Address:              req.IndividualAddress,
        StreetName:           req.StreetName,
        AdditionalStreetName: req.AdditionalStreetName,
        CityName:             req.CityName,
        PostalZone:           req.PostalZone,
        CountrySubentity:     req.CountrySubentity,
        CountryCode:          req.CountryCode,
        BuildingNumber:       req.BuildingNumber,
        CitySubdivisionName:  req.CitySubdivisionName
    );
}
