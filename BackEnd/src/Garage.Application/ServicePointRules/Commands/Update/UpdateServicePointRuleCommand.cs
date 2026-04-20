using Garage.Application.Common;
using Garage.Contracts.ServicePointRules;
using MediatR;

namespace Garage.Application.ServicePointRules.Commands.Update;

public record UpdateServicePointRuleCommand(Guid Id, ServicePointRuleRequest Request) : IRequest<Result<bool>>;
