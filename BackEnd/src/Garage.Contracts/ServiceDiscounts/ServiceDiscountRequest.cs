namespace Garage.Contracts.ServiceDiscounts;

public sealed record ServiceDiscountRequest(
    Guid ServiceId,
    decimal DiscountPercent,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive);
