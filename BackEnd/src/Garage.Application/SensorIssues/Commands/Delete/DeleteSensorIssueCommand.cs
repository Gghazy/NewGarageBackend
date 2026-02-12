using Garage.Application.Common;
using MediatR;
namespace Garage.Application.SensorIssues.Commands.Delete;
public record DeleteSensorIssueCommand(Guid Id) : IRequest<Result<bool>>;

