using Garage.Application.Common;
using Garage.Contracts.ServiceDiscounts;
using MediatR;

namespace Garage.Application.ServiceDiscounts.Commands.Update;

public record UpdateServiceDiscountCommand(Guid Id, ServiceDiscountRequest Request) : IRequest<Result<bool>>;
