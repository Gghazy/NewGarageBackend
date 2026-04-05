using Garage.Domain.Common.Primitives;

namespace Garage.Domain.Bookings.Entities;

public sealed class BookingHistory : Entity
{
    public Guid BookingId { get; private set; }
    public BookingHistoryAction Action { get; private set; }
    public string? Details { get; private set; }

    private BookingHistory() { }

    internal BookingHistory(
        Guid bookingId,
        BookingHistoryAction action,
        string? details = null)
    {
        BookingId = bookingId;
        Action = action;
        Details = details;
    }
}
