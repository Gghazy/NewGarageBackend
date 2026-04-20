using Garage.Application.Common;
using Garage.Contracts.ServicePointRules;
using MediatR;

namespace Garage.Application.ServicePointRules.Commands.Create;

public sealed record CreateServicePointRuleCommand(ServicePointRuleRequest Request) : IRequest<Result<Guid>>;
