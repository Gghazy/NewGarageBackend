using Garage.Application.Common;
using Garage.Contracts.RoadTestIssues;
using MediatR;

namespace Garage.Application.RoadTestIssues.Commands.Create;

public record CreateRoadTestIssueCommand(RoadTestIssueRequest Request) : IRequest<Result<Guid>>;
