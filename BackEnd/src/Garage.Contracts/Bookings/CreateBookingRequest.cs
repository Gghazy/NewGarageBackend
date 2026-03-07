namespace Garage.Contracts.Bookings;

public sealed record CreateBookingRequest(
    Guid    ClientId,
    Guid    BranchId,
    Guid    ManufacturerId,
    Guid    CarMarkId,
    int?    Year,
    string? Transmission,
    DateOnly ExaminationDate,
    TimeOnly ExaminationTime,
    string?  Location,
    string?  Notes
);
