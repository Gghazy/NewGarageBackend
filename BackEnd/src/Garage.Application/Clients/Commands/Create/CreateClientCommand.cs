using Garage.Application.Common;
using Garage.Contracts.Clients;
using MediatR;

namespace Garage.Application.Clients.Commands.Create;

public sealed record CreateClientCommand(CreateClientRequest Request) : IRequest<Result<Guid>>;

