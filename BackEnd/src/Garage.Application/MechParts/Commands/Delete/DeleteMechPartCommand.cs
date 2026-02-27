using Garage.Application.Common;
using MediatR;

namespace Garage.Application.MechParts.Commands.Delete;

public record DeleteMechPartCommand(Guid Id) : IRequest<Result<bool>>;
