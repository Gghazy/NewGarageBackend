using Garage.Application.Common;
using Garage.Contracts.Services;
using Garage.Contracts.Terms;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Terms.Commands.Update
{

    public sealed record UpdateTermCommand(Guid Id, CreateTermsRequest Request) : IRequest<Result<Guid>>;


}
