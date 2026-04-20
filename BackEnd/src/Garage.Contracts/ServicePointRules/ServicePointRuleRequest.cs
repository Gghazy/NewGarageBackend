namespace Garage.Contracts.ServicePointRules;

public sealed record ServicePointRuleRequest(
    decimal FromAmount,
    decimal ToAmount,
    int Points,
    bool IsActive);
