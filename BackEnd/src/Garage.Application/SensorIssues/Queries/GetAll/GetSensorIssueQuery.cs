using Garage.Contracts.Common;
using Garage.Contracts.SensorIssues;
using MediatR;
namespace Garage.Application.SensorIssues.Queries.GetAll;
public record GetSensorIssueQuery(SearchCriteria Search) : IRequest<QueryResult<SensorIssueDto>>;

