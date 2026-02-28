using System.Linq.Expressions;
using Domain.ExaminationManagement.Examinations;
using Garage.Contracts.Examinations;

namespace Garage.Application.Examinations.Queries;

public static class ExaminationProjection
{
    public static readonly Expression<Func<Examination, ExaminationDto>> ToDto =
        e => new ExaminationDto(
            e.Id,
            e.Status.ToString(),
            e.Type.ToString(),
            // Client
            e.Client.ClientId,
            e.Client.NameAr,
            e.Client.NameEn,
            e.Client.PhoneNumber,
            // Branch
            e.Branch.BranchId,
            e.Branch.NameAr,
            e.Branch.NameEn,
            // Vehicle
            e.Vehicle.VehicleId,
            e.Vehicle.ManufacturerId,
            e.Vehicle.ManufacturerNameAr,
            e.Vehicle.ManufacturerNameEn,
            e.Vehicle.CarMarkId,
            e.Vehicle.CarMarkNameAr,
            e.Vehicle.CarMarkNameEn,
            e.Vehicle.Year,
            e.Vehicle.Color,
            e.Vehicle.Vin,
            e.Vehicle.HasPlate,
            e.Vehicle.HasPlate ? e.Vehicle.Plate!.Letters : null,
            e.Vehicle.HasPlate ? e.Vehicle.Plate!.Numbers : null,
            e.Vehicle.Mileage,
            e.Vehicle.MileageUnit.ToString(),
            e.Vehicle.Transmission != null ? e.Vehicle.Transmission.ToString() : null,
            // Meta
            e.HasWarranty,
            e.HasPhotos,
            e.Notes,
            // Items
            e.Items.Select(i => new ExaminationItemDto(
                i.Id,
                i.Service.ServiceId,
                i.Service.NameAr,
                i.Service.NameEn,
                i.Quantity,
                i.OverridePrice,
                i.Status.ToString(),
                i.Notes
            )).ToList(),
            // CreatedAt
            e.CreatedAtUtc
        );
}
