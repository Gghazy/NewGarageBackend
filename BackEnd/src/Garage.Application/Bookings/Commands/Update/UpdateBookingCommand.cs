using Garage.Application.Common;
using Garage.Contracts.Bookings;
using MediatR;

namespace Garage.Application.Bookings.Commands.Update;

public record UpdateBookingCommand(Guid Id, UpdateBookingRequest Request) : IRequest<Result<bool>>;
