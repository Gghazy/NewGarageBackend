using Garage.Application.Common;
using MediatR;

namespace Garage.Application.ServiceDiscounts.Commands.Delete;

public record DeleteServiceDiscountCommand(Guid Id) : IRequest<Result<bool>>;
