using Cashif.Application.Common;
using Cashif.Contracts.Branches;
using MediatR;
namespace Cashif.Application.Branches.Commands.Create;
public record CreateBranchCommand(CreateBranchRequest Request) : IRequest<Result<Guid>>;
