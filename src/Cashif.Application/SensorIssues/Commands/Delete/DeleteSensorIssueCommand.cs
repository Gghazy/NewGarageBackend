using Cashif.Application.Common;
using MediatR;
namespace Cashif.Application.SensorIssues.Commands.Delete;
public record DeleteSensorIssueCommand(Guid Id) : IRequest<Result<bool>>;
