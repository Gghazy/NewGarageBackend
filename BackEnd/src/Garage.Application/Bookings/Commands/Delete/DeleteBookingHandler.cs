using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Bookings.Entities;

namespace Garage.Application.Bookings.Commands.Delete;

public sealed class DeleteBookingHandler(
    IRepository<Booking> bookingRepo,
    IUnitOfWork          unitOfWork)
    : BaseCommandHandler<DeleteBookingCommand, bool>
{
    public override async Task<Result<bool>> Handle(DeleteBookingCommand command, CancellationToken ct)
    {
        var booking = await bookingRepo.GetByIdAsync(command.Id, ct);
        if (booking is null) return Fail(NotFoundError);

        booking.MarkDeleted();
        await bookingRepo.SoftDeleteAsync(booking, ct: ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
