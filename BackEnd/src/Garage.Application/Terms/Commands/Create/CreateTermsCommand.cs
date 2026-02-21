using Garage.Application.Common;
using Garage.Contracts.Terms;
using MediatR;

namespace Garage.Application.Terms.Commands.Create
{
    public sealed record CreateTermCommand(CreateTermsRequest Request) : IRequest<Result<Guid>>;

}
