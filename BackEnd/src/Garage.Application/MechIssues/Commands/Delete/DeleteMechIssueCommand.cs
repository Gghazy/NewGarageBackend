using Garage.Application.Common;
using MediatR;

namespace Garage.Application.MechIssues.Commands.Delete;

public record DeleteMechIssueCommand(Guid Id) : IRequest<Result<bool>>;
