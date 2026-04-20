namespace Garage.Contracts.ServiceDiscounts;

public sealed record ServiceDiscountDto(
    Guid Id,
    Guid ServiceId,
    string ServiceNameAr,
    string ServiceNameEn,
    decimal DiscountPercent,
    DateTime StartDate,
    DateTime EndDate,
    bool IsActive);
