using Garage.Application.Abstractions;
using Garage.Contracts.Bookings;
using Garage.Domain.Bookings.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Bookings.Queries.GetByExaminationId;

public sealed class GetBookingByExaminationIdQueryHandler(IReadRepository<Booking> repo)
    : IRequestHandler<GetBookingByExaminationIdQuery, BookingDto?>
{
    public async Task<BookingDto?> Handle(GetBookingByExaminationIdQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Where(b => b.ConvertedExaminationId == request.ExaminationId)
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
