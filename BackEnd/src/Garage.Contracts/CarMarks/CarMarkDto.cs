namespace Garage.Contracts.CarMarks;

public record CarMarkDto(
    Guid Id,
    string NameAr,
    string NameEn,
    Guid ManufacturerId,
    string ManufacturerNameAr,
    string ManufacturerNameEn);
