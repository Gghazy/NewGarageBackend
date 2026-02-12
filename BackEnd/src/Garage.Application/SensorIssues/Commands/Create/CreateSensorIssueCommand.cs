using Garage.Application.Common;
using Garage.Contracts.SensorIssues;
using MediatR;
namespace Garage.Application.SensorIssues.Commands.Create;
public record CreateSensorIssueCommand(CreateSensorIssueRequest Request) : IRequest<Result<Guid>>;

