using Garage.Application.Common;
using MediatR;

namespace Garage.Application.ServicePointRules.Commands.Delete;

public record DeleteServicePointRuleCommand(Guid Id) : IRequest<Result<bool>>;
