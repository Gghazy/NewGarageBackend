using Garage.Application.Common;
using Garage.Contracts.MechParts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechParts.Commands.Update
{

    public record UpdateMechPartCommand(Guid Id, MechPartRequest Request) : IRequest<Result<bool>>;

}
