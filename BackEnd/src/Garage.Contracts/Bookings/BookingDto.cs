namespace Garage.Contracts.Bookings;

public sealed record BookingDto(
    Guid    Id,
    Guid    ClientId,
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,
    Guid    BranchId,
    string  BranchNameAr,
    string  BranchNameEn,
    Guid    ManufacturerId,
    string  ManufacturerNameAr,
    string  ManufacturerNameEn,
    Guid    CarMarkId,
    string  CarMarkNameAr,
    string  CarMarkNameEn,
    int?    Year,
    string? Transmission,
    DateOnly ExaminationDate,
    TimeOnly ExaminationTime,
    string?  Location,
    string?  Notes,
    string   Status,
    Guid?    ConvertedExaminationId,
    DateTime CreatedAtUtc
);
