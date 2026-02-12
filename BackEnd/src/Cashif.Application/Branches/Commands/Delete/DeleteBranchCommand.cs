using Cashif.Application.Common;
using MediatR;
namespace Cashif.Application.Branches.Commands.Delete;
public record DeleteBranchCommand(Guid Id) : IRequest<Result<bool>>;
