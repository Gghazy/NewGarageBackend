using Garage.Contracts.Clients;
using MediatR;


namespace Garage.Application.Clients.Commands.Update;

public sealed record UpdateClientCommand(Guid Id, CreateClientRequest Request) : IRequest<Guid>;
