using Cashif.Application.Common;
using Cashif.Contracts.SensorIssues;
using MediatR;
namespace Cashif.Application.SensorIssues.Commands.Create;
public record CreateSensorIssueCommand(CreateSensorIssueRequest Request) : IRequest<Result<Guid>>;
