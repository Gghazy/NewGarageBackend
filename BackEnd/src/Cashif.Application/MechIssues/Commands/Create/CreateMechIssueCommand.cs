using Cashif.Application.Common;
using Cashif.Contracts.Branches;
using Cashif.Contracts.MechIssues;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.MechIssues.Commands.Create
{

    public record CreateMechIssueCommand(MechIssueRequest Request) : IRequest<Result<Guid>>;

}
