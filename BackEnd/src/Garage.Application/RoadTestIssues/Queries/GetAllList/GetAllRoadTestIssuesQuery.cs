using Garage.Contracts.RoadTestIssues;
using MediatR;

namespace Garage.Application.RoadTestIssues.Queries.GetAllList;

public record GetAllRoadTestIssuesQuery() : IRequest<List<RoadTestIssueResponse>>;
