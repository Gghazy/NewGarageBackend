using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.ServicePrices
{
    public sealed record ServicePriceDto(
    Guid Id,
    Guid ServiceId,
    string ServiceNameAr,
    string ServiceNameEn,
    Guid MarkId,
    string BrandNameAr,
    string BrandNameEn,
    int FromYear,
    int ToYear,
    decimal Price);
}
