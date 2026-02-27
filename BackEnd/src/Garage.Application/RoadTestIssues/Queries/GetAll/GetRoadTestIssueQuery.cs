using Garage.Contracts.Common;
using Garage.Contracts.RoadTestIssues;
using MediatR;

namespace Garage.Application.RoadTestIssues.Queries.GetAll;

public record GetRoadTestIssueQuery(SearchCriteria Search) : IRequest<QueryResult<RoadTestIssueResponse>>;
