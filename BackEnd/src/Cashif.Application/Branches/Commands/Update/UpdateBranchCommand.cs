using Cashif.Application.Common;
using Cashif.Contracts.Branches;
using MediatR;
namespace Cashif.Application.Branches.Commands.Update;
public record UpdateBranchCommand(Guid Id, UpdateBranchRequest Request) : IRequest<Result<bool>>;
