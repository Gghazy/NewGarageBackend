using Garage.Contracts.Common;


namespace Garage.Contracts.ServicePrices
{
    public sealed record ServicePriceFilterDto(
      SearchCriteria Search,
      Guid? ServiceId,
      Guid? MarkId,
      int? Year
    );
}
