using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Bookings.Entities;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Manufacturers.Entity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Bookings.Commands.Update;

public sealed class UpdateBookingHandler(
    IRepository<Booking>           bookingRepo,
    IReadRepository<Client>        clientRepo,
    IReadRepository<Branch>        branchRepo,
    IReadRepository<Manufacturer>  manufacturerRepo,
    IReadRepository<CarMark>       carMarkRepo,
    IUnitOfWork                    unitOfWork)
    : BaseCommandHandler<UpdateBookingCommand, bool>
{
    public override async Task<Result<bool>> Handle(UpdateBookingCommand command, CancellationToken ct)
    {
        var booking = await bookingRepo.GetByIdAsync(command.Id, ct);
        if (booking is null) return Fail(NotFoundError);

        var req = command.Request;

        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == req.ClientId, ct);
        if (client is null) return Fail("Client not found.");

        var branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        var manufacturer = await manufacturerRepo.Query()
            .Where(m => m.Id == req.ManufacturerId)
            .Select(m => new { m.NameAr, m.NameEn })
            .FirstOrDefaultAsync(ct);
        if (manufacturer is null) return Fail("Manufacturer not found.");

        var carMark = await carMarkRepo.Query()
            .Where(c => c.Id == req.CarMarkId)
            .Select(c => new { c.NameAr, c.NameEn })
            .FirstOrDefaultAsync(ct);
        if (carMark is null) return Fail("Car mark not found.");

        booking.Update(
            clientId: client.Id,
            clientNameAr: client.NameAr,
            clientNameEn: client.NameEn,
            clientPhone: client.PhoneNumber,
            branchId: branch.Id,
            branchNameAr: branch.NameAr,
            branchNameEn: branch.NameEn,
            manufacturerId: req.ManufacturerId,
            manufacturerNameAr: manufacturer.NameAr,
            manufacturerNameEn: manufacturer.NameEn,
            carMarkId: req.CarMarkId,
            carMarkNameAr: carMark.NameAr,
            carMarkNameEn: carMark.NameEn,
            year: req.Year,
            transmission: req.Transmission,
            examinationDate: req.ExaminationDate,
            examinationTime: req.ExaminationTime,
            location: req.Location,
            notes: req.Notes);

        await bookingRepo.UpdateAsync(booking, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
