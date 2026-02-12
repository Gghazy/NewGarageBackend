using Garage.Application.Common;
using MediatR;
namespace Garage.Application.Branches.Commands.Delete;
public record DeleteBranchCommand(Guid Id) : IRequest<Result<bool>>;

