using Garage.Application.Common;
using Garage.Contracts.Terms;
using MediatR;

namespace Garage.Application.Terms.Commands.Update
{
    public sealed record UpdateTermCommand(Guid Id, CreateTermsRequest Request) : IRequest<Result<Guid>>;
}
