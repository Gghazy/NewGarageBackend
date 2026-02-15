using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.ServicePrices
{
    public sealed record ServicePriceRequest(Guid ServiceId, Guid MarkId, int FromYear, int ToYear, decimal Price);
}
