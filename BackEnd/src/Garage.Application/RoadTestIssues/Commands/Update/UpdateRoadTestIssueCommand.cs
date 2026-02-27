using Garage.Application.Common;
using Garage.Contracts.RoadTestIssues;
using MediatR;

namespace Garage.Application.RoadTestIssues.Commands.Update;

public record UpdateRoadTestIssueCommand(Guid Id, RoadTestIssueRequest Request) : IRequest<Result<bool>>;
