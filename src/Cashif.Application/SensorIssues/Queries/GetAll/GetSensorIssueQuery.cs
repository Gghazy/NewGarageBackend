using Cashif.Contracts.Common;
using Cashif.Contracts.SensorIssues;
using MediatR;
namespace Cashif.Application.SensorIssues.Queries.GetAll;
public record GetSensorIssueQuery(SearchCriteria Search) : IRequest<QueryResult<SensorIssueDto>>;
