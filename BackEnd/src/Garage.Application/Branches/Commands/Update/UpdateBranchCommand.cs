using Garage.Application.Common;
using Garage.Contracts.Branches;
using MediatR;
namespace Garage.Application.Branches.Commands.Update;
public record UpdateBranchCommand(Guid Id, UpdateBranchRequest Request) : IRequest<Result<bool>>;

