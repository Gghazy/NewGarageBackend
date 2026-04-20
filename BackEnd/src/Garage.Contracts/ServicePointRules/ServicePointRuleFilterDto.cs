using Garage.Contracts.Common;

namespace Garage.Contracts.ServicePointRules;

public sealed record ServicePointRuleFilterDto(
    SearchCriteria Search,
    bool? IsActive);
