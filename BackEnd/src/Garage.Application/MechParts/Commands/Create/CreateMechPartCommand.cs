using Garage.Application.Common;
using Garage.Contracts.MechParts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechParts.Commands.Create
{

    public record CreateMechPartCommand(MechPartRequest Request) : IRequest<Result<Guid>>;

}
