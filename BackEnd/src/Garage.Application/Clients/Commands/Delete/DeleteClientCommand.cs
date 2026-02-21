using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Clients.Commands.Delete;

public record DeleteClientCommand(Guid Id) : IRequest<Result<bool>>;
