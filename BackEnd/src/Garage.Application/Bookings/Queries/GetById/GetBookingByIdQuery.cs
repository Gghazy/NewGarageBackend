using Garage.Contracts.Bookings;
using MediatR;

namespace Garage.Application.Bookings.Queries.GetById;

public record GetBookingByIdQuery(Guid Id) : IRequest<BookingDto?>;
