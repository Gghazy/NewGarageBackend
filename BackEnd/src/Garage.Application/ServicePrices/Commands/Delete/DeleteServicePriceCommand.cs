using Garage.Application.Common;
using MediatR;

namespace Garage.Application.ServicePrices.Commands.Delete;

public record DeleteServicePriceCommand(Guid Id) : IRequest<Result<bool>>;
