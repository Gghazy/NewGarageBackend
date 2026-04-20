using Garage.Application.Common;
using Garage.Contracts.ServiceDiscounts;
using MediatR;

namespace Garage.Application.ServiceDiscounts.Commands.Create;

public sealed record CreateServiceDiscountCommand(ServiceDiscountRequest Request) : IRequest<Result<Guid>>;
