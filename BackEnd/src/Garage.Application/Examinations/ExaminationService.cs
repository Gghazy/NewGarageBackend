using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Contracts.Clients;
using Garage.Contracts.Examinations;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.ServicePrices.Entities;
using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations;

public sealed class ExaminationService(
    IReadRepository<Service>      serviceRepo,
    IReadRepository<ServicePrice> servicePriceRepo,
    IReadRepository<Manufacturer> manufacturerRepo,
    IReadRepository<CarMark>      carMarkRepo)
{
    // ── Parse MileageUnit + TransmissionType ────────────────────────
    public Result<(MileageUnit MileageUnit, TransmissionType? Transmission)>
        ParseVehicleEnums(IExaminationRequest req)
    {
        if (!Enum.TryParse<MileageUnit>(req.MileageUnit, ignoreCase: true, out var mileageUnit))
            return Result<(MileageUnit, TransmissionType?)>
                .Fail($"Invalid mileage unit '{req.MileageUnit}'. Use Km or Mile.");

        TransmissionType? transmission = null;
        if (!string.IsNullOrWhiteSpace(req.Transmission))
        {
            if (!Enum.TryParse<TransmissionType>(req.Transmission, ignoreCase: true, out var tp))
                return Result<(MileageUnit, TransmissionType?)>
                    .Fail($"Invalid transmission type '{req.Transmission}'.");
            transmission = tp;
        }

        return Result<(MileageUnit, TransmissionType?)>.Ok((mileageUnit, transmission));
    }

    // ── Build PlateNumber ───────────────────────────────────────────
    public Result<PlateNumber?> BuildPlate(IExaminationRequest req)
    {
        if (!req.HasPlate)
            return Result<PlateNumber?>.Ok(null);

        if (string.IsNullOrWhiteSpace(req.PlateLetters) ||
            string.IsNullOrWhiteSpace(req.PlateNumbers))
            return Result<PlateNumber?>.Fail(
                "Plate letters and numbers are required when vehicle has a plate.");

        return Result<PlateNumber?>.Ok(PlateNumber.Create(req.PlateLetters, req.PlateNumbers));
    }

    // ── Load & validate Manufacturer + CarMark ──────────────────────
    public async Task<Result<(Manufacturer Manufacturer, CarMark CarMark)>>
        LoadVehicleLookupsAsync(IExaminationRequest req, CancellationToken ct)
    {
        var manufacturer = await manufacturerRepo.GetByIdAsync(req.ManufacturerId, ct);
        if (manufacturer is null)
            return Result<(Manufacturer, CarMark)>.Fail("Manufacturer not found.");

        var carMark = await carMarkRepo.GetByIdAsync(req.CarMarkId, ct);
        if (carMark is null)
            return Result<(Manufacturer, CarMark)>.Fail("Car mark not found.");

        return Result<(Manufacturer, CarMark)>.Ok((manufacturer, carMark));
    }

    // ── Load & validate Services ────────────────────────────────────
    public async Task<Result<List<Service>>> LoadServicesAsync(
        IExaminationRequest req, CancellationToken ct)
    {
        if (req.Items is not { Count: > 0 })
            return Result<List<Service>>.Ok([]);

        var serviceIds = req.Items.Select(i => i.ServiceId).Distinct().ToList();
        var services = await serviceRepo.Query()
            .Where(s => serviceIds.Contains(s.Id))
            .ToListAsync(ct);

        return services.Count != serviceIds.Count
            ? Result<List<Service>>.Fail("One or more services were not found.")
            : Result<List<Service>>.Ok(services);
    }

    // ── Add items to examination (no pricing – operational only) ─────
    public static void AddItems(
        Examination examination,
        IExaminationRequest req,
        List<Service> services)
    {
        if (req.Items is not { Count: > 0 })
            return;

        var serviceMap = services.ToDictionary(s => s.Id);

        foreach (var itemReq in req.Items)
        {
            var svc      = serviceMap[itemReq.ServiceId];
            var snapshot = new ServiceSnapshot(svc.Id, svc.NameAr, svc.NameEn);
            examination.AddItem(snapshot, itemReq.Quantity, itemReq.OverridePrice);
        }
    }

    // ── Lookup ServicePrices for invoice item creation ────────────────
    public async Task<Dictionary<Guid, decimal>> LookupServicePricesAsync(
        IEnumerable<Guid> serviceIds,
        Guid carMarkId,
        int? year,
        CancellationToken ct)
    {
        var ids = serviceIds.Distinct().ToList();

        var servicePrices = await servicePriceRepo.Query()
            .Where(sp => ids.Contains(sp.ServiceId)
                      && sp.MarkId == carMarkId
                      && (!year.HasValue
                          || (sp.FromYear <= year.Value && sp.ToYear >= year.Value)))
            .ToListAsync(ct);

        return servicePrices.ToDictionary(sp => sp.ServiceId, sp => sp.Price);
    }

    // ── Map examination request to client request ───────────────────
    public static CreateClientRequest MapToClientRequest(IExaminationRequest req) => new(
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
