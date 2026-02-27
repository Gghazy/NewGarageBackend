using Garage.Application.Common;
using MediatR;

namespace Garage.Application.RoadTestIssues.Commands.Delete;

public record DeleteRoadTestIssueCommand(Guid Id) : IRequest<Result<bool>>;
