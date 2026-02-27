using Garage.Contracts.SensorIssues;
using MediatR;

namespace Garage.Application.SensorIssues.Queries.GetAllList;

public record GetAllSensorIssuesQuery() : IRequest<List<SensorIssueDto>>;
