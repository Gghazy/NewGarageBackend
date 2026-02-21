using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Terms.Commands.Delete;

public record DeleteTermCommand(Guid Id) : IRequest<Result<bool>>;
