using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Services.Commands.Delete;

public record DeleteServiceCommand(Guid Id) : IRequest<Result<bool>>;
