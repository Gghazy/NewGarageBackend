using Garage.Contracts.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Services.Commands.UpsertServicePrice
{
    public sealed record UpsertServicePriceCommand(
       Guid ServiceId,
       ServicePriceRequest Request
   ) : IRequest;
}
