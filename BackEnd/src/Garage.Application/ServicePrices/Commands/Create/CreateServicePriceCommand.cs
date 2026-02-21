using Garage.Application.Common;
using Garage.Contracts.ServicePrices;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.ServicePrices.Commands.Create
{
        public sealed record CreateServicePriceCommand(ServicePriceRequest Request) : IRequest<Result<Guid>>;

}
