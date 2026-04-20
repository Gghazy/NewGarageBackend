namespace Garage.Contracts.ServicePointRules;

public sealed record ServicePointRuleDto(
    Guid Id,
    decimal FromAmount,
    decimal ToAmount,
    int Points,
    bool IsActive);
