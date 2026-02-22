using Garage.Domain.ExaminationManagement.Shared;


namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed record VehicleSnapshot(
 Guid VehicleId,
 PlateNumber PlateNumber,
 string ManufacturerName,
 string ModelName,
 int? Year,
 string? Color,
 string? Vin
);
