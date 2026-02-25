using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Clients.Commands.Update;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Clients;
using Garage.Contracts.Examinations;
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

public sealed class UpdateExaminationHandler : BaseCommandHandler<UpdateExaminationCommand, Guid>
{
    private readonly IRepository<Examination>      _examinationRepo;
    private readonly IRepository<Vehicle>          _vehicleRepo;
    private readonly IReadRepository<Client>       _clientRepo;
    private readonly IReadRepository<Service>      _serviceRepo;
    private readonly IReadRepository<Manufacturer> _manufacturerRepo;
    private readonly IReadRepository<CarMark>      _carMarkRepo;
    private readonly IMediator                     _mediator;
    private readonly IUnitOfWork                   _unitOfWork;

    public UpdateExaminationHandler(
        IRepository<Examination>      examinationRepo,
        IRepository<Vehicle>          vehicleRepo,
        IReadRepository<Client>       clientRepo,
        IReadRepository<Service>      serviceRepo,
        IReadRepository<Manufacturer> manufacturerRepo,
        IReadRepository<CarMark>      carMarkRepo,
        IMediator                     mediator,
        IUnitOfWork                   unitOfWork)
    {
        _examinationRepo  = examinationRepo;
        _vehicleRepo      = vehicleRepo;
        _clientRepo       = clientRepo;
        _serviceRepo      = serviceRepo;
        _manufacturerRepo = manufacturerRepo;
        _carMarkRepo      = carMarkRepo;
        _mediator         = mediator;
        _unitOfWork       = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(UpdateExaminationCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // ── 1. Load examination ───────────────────────────────────────────────
        var examination = await _examinationRepo.QueryTracking()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.Id, ct);

        if (examination is null)
            return Fail("Examination not found.");

        // ── 2. Lookups (before any writes) ────────────────────────────────────
        var manufacturer = await _manufacturerRepo.GetByIdAsync(req.ManufacturerId, ct);
        if (manufacturer is null) return Fail("Manufacturer not found.");

        var carMark = await _carMarkRepo.GetByIdAsync(req.CarMarkId, ct);
        if (carMark is null) return Fail("Car mark not found.");

        // ── 3. Parse enums ────────────────────────────────────────────────────
        if (!Enum.TryParse<MileageUnit>(req.MileageUnit, ignoreCase: true, out var mileageUnit))
            return Fail($"Invalid mileage unit '{req.MileageUnit}'. Use Km or Mile.");

        TransmissionType? transmission = null;
        if (!string.IsNullOrWhiteSpace(req.Transmission))
        {
            if (!Enum.TryParse<TransmissionType>(req.Transmission, ignoreCase: true, out var tp))
                return Fail($"Invalid transmission type '{req.Transmission}'.");
            transmission = tp;
        }

        // ── 4. Build plate ────────────────────────────────────────────────────
        PlateNumber? plate = null;
        if (req.HasPlate)
        {
            if (string.IsNullOrWhiteSpace(req.PlateLetters) || string.IsNullOrWhiteSpace(req.PlateNumbers))
                return Fail("Plate letters and numbers are required when vehicle has a plate.");
            plate = PlateNumber.Create(req.PlateLetters, req.PlateNumbers);
        }

        // ── 5. Load services if items provided ────────────────────────────────
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

        // ── 6. All writes inside one transaction ──────────────────────────────
        await using var tx = await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
            // 6a. Update client via existing UpdateClientCommand ───────────────
            var clientId = examination.Client.ClientId;
            var updateClientResult = await _mediator.Send(
                new UpdateClientCommand(clientId, MapToClientRequest(req)), ct);

            if (!updateClientResult.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return Fail(updateClientResult.Error!);
            }

            // Reload updated client to get fresh data
            var client = await _clientRepo.QueryTracking()
                .FirstOrDefaultAsync(c => c.Id == clientId, ct);

            if (client is null)
            {
                await tx.RollbackAsync(ct);
                return Fail("Client not found after update.");
            }

            // 6b. Update Vehicle entity ────────────────────────────────────────
            var vehicleId = examination.Vehicle.VehicleId;
            var vehicle   = await _vehicleRepo.QueryTracking()
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
                plate:              plate,
                mileage:            req.Mileage,
                mileageUnit:        mileageUnit,
                transmission:       transmission);

            // 6c. Update examination ───────────────────────────────────────────
            examination.Update(req.HasWarranty, req.HasPhotos, req.MarketerCode, req.Notes);

            examination.UpdateClientSnapshot(new ClientReference(
                ClientId:    client.Id,
                NameAr:      client.NameAr,
                NameEn:      client.NameEn,
                PhoneNumber: client.PhoneNumber,
                Email:       req.ClientEmail));

            examination.UpdateVehicleSnapshot(vehicle.ToSnapshot());

            // 6d. Replace items if provided (Draft only — domain enforces this) ─
            if (req.Items != null && req.Items.Count > 0)
            {
                if (examination.Status != ExaminationStatus.Draft)
                    throw new InvalidOperationException("Items can only be replaced in Draft status.");

                foreach (var item in examination.Items.ToList())
                    examination.RemoveItem(item.Service.ServiceId);

                var serviceMap = services.ToDictionary(s => s.Id);
                foreach (var itemReq in req.Items)
                {
                    var svc      = serviceMap[itemReq.ServiceId];
                    var snapshot = new ServiceSnapshot(svc.Id, svc.NameAr, svc.NameEn, Money.Zero());
                    var price    = itemReq.OverridePrice.HasValue ? Money.Create(itemReq.OverridePrice.Value) : null;
                    examination.AddItem(snapshot, price);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Ok(examination.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Fail($"Failed to update examination: {ex.Message}");
        }
    }

    private static CreateClientRequest MapToClientRequest(UpdateExaminationRequest req) => new(
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
