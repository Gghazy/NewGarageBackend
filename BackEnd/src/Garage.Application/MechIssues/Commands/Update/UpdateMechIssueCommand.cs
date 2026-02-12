using Garage.Application.Common;
using Garage.Contracts.Branches;
using Garage.Contracts.MechIssues;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.MechIssues.Commands.Update
{

    public record UpdateMechIssueCommand(Guid Id, MechIssueRequest Request) : IRequest<Result<bool>>;

}

