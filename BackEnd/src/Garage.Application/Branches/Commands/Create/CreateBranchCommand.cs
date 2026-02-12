using Garage.Application.Common;
using Garage.Contracts.Branches;
using MediatR;
namespace Garage.Application.Branches.Commands.Create;
public record CreateBranchCommand(CreateBranchRequest Request) : IRequest<Result<Guid>>;

