using Garage.Contracts.Common;
using Garage.Contracts.ServicePrices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.ServicePrices.Queries.GetAllServicePriceBySearch
{
    public sealed record GetAllServicePriceBySearchQuery(ServicePriceFilterDto Request) : IRequest<QueryResult<ServicePriceDto>>;
}
