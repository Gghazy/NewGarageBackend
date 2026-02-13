using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Services
{
    public sealed record ServicePriceDto(Guid MarkId, int FromYear, int ToYear, decimal Price);
}
