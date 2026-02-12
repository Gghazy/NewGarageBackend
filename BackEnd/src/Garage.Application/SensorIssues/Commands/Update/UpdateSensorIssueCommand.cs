using Garage.Application.Common;
using Garage.Contracts.SensorIssues;
using MediatR;
namespace Garage.Application.SensorIssues.Commands.Update;
public record UpdateSensorIssueCommand(Guid Id, UpdateSensorIssueRequest Request) : IRequest<Result<bool>>;

