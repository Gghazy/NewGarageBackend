using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Bookings.Entities;

namespace Garage.Application.Bookings.Commands.ChangeStatus;

public sealed class ChangeBookingStatusHandler(
    IRepository<Booking> bookingRepo,
    IUnitOfWork          unitOfWork)
    : BaseCommandHandler<ChangeBookingStatusCommand, bool>
{
    public override async Task<Result<bool>> Handle(ChangeBookingStatusCommand command, CancellationToken ct)
    {
        var booking = await bookingRepo.GetByIdAsync(command.Id, ct);
        if (booking is null) return Fail(NotFoundError);

        switch (command.Action.ToLowerInvariant())
        {
            case "confirm":
                booking.Confirm();
                break;
            case "cancel":
                booking.Cancel();
                break;
            default:
                return Fail($"Unknown action '{command.Action}'.");
        }

        await bookingRepo.UpdateAsync(booking, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
