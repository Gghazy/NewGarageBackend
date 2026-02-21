using Garage.Application.Common;
using Garage.Contracts.ServicePrices;
using MediatR;


namespace Garage.Application.ServicePrices.Commands.Update
{

    public record UpdateServicePriceCommand(Guid Id, ServicePriceRequest Request) : IRequest<Result<Guid>>;


}
