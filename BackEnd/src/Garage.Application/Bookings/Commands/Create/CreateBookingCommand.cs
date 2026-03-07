using Garage.Application.Common;
using Garage.Contracts.Bookings;
using MediatR;

namespace Garage.Application.Bookings.Commands.Create;

public record CreateBookingCommand(CreateBookingRequest Request) : IRequest<Result<Guid>>;
