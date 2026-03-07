using Garage.Application.Abstractions;
using Garage.Contracts.Bookings;
using Garage.Domain.Bookings.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Bookings.Queries.GetById;

public sealed class GetBookingByIdQueryHandler(IReadRepository<Booking> repo)
    : IRequestHandler<GetBookingByIdQuery, BookingDto?>
{
    public async Task<BookingDto?> Handle(GetBookingByIdQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Where(b => b.Id == request.Id)
            .Select(b => new BookingDto(
                b.Id,
                b.ClientId, b.ClientNameAr, b.ClientNameEn, b.ClientPhone,
                b.BranchId, b.BranchNameAr, b.BranchNameEn,
                b.ManufacturerId, b.ManufacturerNameAr, b.ManufacturerNameEn,
                b.CarMarkId, b.CarMarkNameAr, b.CarMarkNameEn,
                b.Year, b.Transmission,
                b.ExaminationDate, b.ExaminationTime,
                b.Location, b.Notes,
                b.Status.ToString(),
                b.ConvertedExaminationId,
                b.CreatedAtUtc))
            .FirstOrDefaultAsync(ct);
    }
}
