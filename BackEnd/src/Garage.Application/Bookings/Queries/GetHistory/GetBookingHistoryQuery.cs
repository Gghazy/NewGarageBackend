using Garage.Contracts.Bookings;
using MediatR;

namespace Garage.Application.Bookings.Queries.GetHistory;

public sealed record GetBookingHistoryQuery(Guid BookingId)
    : IRequest<List<BookingHistoryDto>>;
