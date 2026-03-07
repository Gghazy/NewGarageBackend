using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Bookings.Commands.Convert;

public record ConvertBookingCommand(Guid BookingId) : IRequest<Result<Guid>>;
