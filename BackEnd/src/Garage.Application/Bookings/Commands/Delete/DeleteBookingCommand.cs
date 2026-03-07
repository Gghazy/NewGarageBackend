using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Bookings.Commands.Delete;

public record DeleteBookingCommand(Guid Id) : IRequest<Result<bool>>;
