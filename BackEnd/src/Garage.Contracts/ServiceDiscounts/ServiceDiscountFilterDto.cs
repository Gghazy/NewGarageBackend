using Garage.Contracts.Common;

namespace Garage.Contracts.ServiceDiscounts;

public sealed record ServiceDiscountFilterDto(
    SearchCriteria Search,
    Guid? ServiceId,
    bool? IsActive);
