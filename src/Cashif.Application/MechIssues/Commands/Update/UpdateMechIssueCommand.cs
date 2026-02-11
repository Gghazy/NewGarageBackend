using Cashif.Application.Common;
using Cashif.Contracts.Branches;
using Cashif.Contracts.MechIssues;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.MechIssues.Commands.Update
{

    public record UpdateMechIssueCommand(Guid Id, MechIssueRequest Request) : IRequest<Result<bool>>;

}
