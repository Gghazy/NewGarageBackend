using Garage.Contracts.Bookings;
using MediatR;

namespace Garage.Application.Bookings.Queries.GetByExaminationId;

public sealed record GetBookingByExaminationIdQuery(Guid ExaminationId)
    : IRequest<BookingDto?>;
