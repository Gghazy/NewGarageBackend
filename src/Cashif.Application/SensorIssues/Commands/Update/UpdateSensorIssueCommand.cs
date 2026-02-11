using Cashif.Application.Common;
using Cashif.Contracts.SensorIssues;
using MediatR;
namespace Cashif.Application.SensorIssues.Commands.Update;
public record UpdateSensorIssueCommand(Guid Id, UpdateSensorIssueRequest Request) : IRequest<Result<bool>>;
