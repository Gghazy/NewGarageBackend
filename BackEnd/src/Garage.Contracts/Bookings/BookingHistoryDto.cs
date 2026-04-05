namespace Garage.Contracts.Bookings;

public sealed record BookingHistoryDto(
    Guid Id,
    string Action,
    string? Details,
    Guid? PerformedById,
    string? PerformedByNameAr,
    string? PerformedByNameEn,
    DateTime CreatedAtUtc
);
