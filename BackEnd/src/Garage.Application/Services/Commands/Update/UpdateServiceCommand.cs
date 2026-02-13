using Garage.Contracts.Services;
using MediatR;


namespace Garage.Application.Services.Commands.Update
{
    public sealed record UpdateServiceCommand(Guid Id,CreateServiceRequest Request) : IRequest<Guid>;

}
