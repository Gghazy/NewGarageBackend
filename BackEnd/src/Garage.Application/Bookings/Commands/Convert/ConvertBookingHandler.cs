using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Bookings.Entities;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Domain.Manufacturers.Entity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Bookings.Commands.Convert;

public sealed class ConvertBookingHandler(
    IRepository<Booking>           bookingRepo,
    IRepository<Examination>       examinationRepo,
    IRepository<Vehicle>           vehicleRepo,
    IReadRepository<Client>        clientRepo,
    IReadRepository<Branch>        branchRepo,
    IReadRepository<Manufacturer>  manufacturerRepo,
    IReadRepository<CarMark>       carMarkRepo,
    IUnitOfWork                    unitOfWork)
    : BaseCommandHandler<ConvertBookingCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(ConvertBookingCommand command, CancellationToken ct)
    {
        var booking = await bookingRepo.GetByIdAsync(command.BookingId, ct);
        if (booking is null) return Fail(NotFoundError);

        if (booking.Status == BookingStatus.Converted)
            return Fail("Booking is already converted.");
        if (booking.Status == BookingStatus.Cancelled)
            return Fail("Cancelled booking cannot be converted.");

        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == booking.ClientId, ct);
        if (client is null) return Fail("Client not found.");

        var branch = await branchRepo.GetByIdAsync(booking.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        var manufacturer = await manufacturerRepo.Query()
            .Where(m => m.Id == booking.ManufacturerId)
            .Select(m => new { m.NameAr, m.NameEn })
            .FirstOrDefaultAsync(ct);
        if (manufacturer is null) return Fail("Manufacturer not found.");

        var carMark = await carMarkRepo.Query()
            .Where(c => c.Id == booking.CarMarkId)
            .Select(c => new { c.NameAr, c.NameEn })
            .FirstOrDefaultAsync(ct);
        if (carMark is null) return Fail("Car mark not found.");

        // Parse transmission
        TransmissionType? transmission = null;
        if (!string.IsNullOrWhiteSpace(booking.Transmission)
            && Enum.TryParse<TransmissionType>(booking.Transmission, ignoreCase: true, out var parsed))
            transmission = parsed;

        var clientRef = new ClientReference(
            ClientId: client.Id,
            NameAr: client.NameAr,
            NameEn: client.NameEn,
            PhoneNumber: client.PhoneNumber,
            Email: null);

        var branchRef = new BranchReference(
            BranchId: branch.Id,
            NameAr: branch.NameAr,
            NameEn: branch.NameEn);

        // Create vehicle
        var vehicle = Vehicle.Create(
            manufacturerId: booking.ManufacturerId,
            manufacturerNameAr: manufacturer.NameAr,
            manufacturerNameEn: manufacturer.NameEn,
            carMarkId: booking.CarMarkId,
            carMarkNameAr: carMark.NameAr,
            carMarkNameEn: carMark.NameEn,
            year: booking.Year,
            color: null,
            vin: null,
            hasPlate: false,
            plate: null,
            mileage: null,
            mileageUnit: MileageUnit.Km,
            transmission: transmission);

        var vehicleSnapshot = vehicle.ToSnapshot();

        await using var tx = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await vehicleRepo.AddAsync(vehicle, ct);

            var examination = Examination.Create(
                client: clientRef,
                branch: branchRef,
                vehicle: vehicleSnapshot,
                type: ExaminationType.Regular,
                hasWarranty: false,
                hasPhotos: false);

            await examinationRepo.AddAsync(examination, ct);

            booking.MarkConverted(examination.Id);
            await bookingRepo.UpdateAsync(booking, ct);

            await unitOfWork.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            return Ok(examination.Id);
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync(ct);
            return Fail($"Failed to convert booking: {ex.Message}");
        }
    }
}
