using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Bookings;
using Garage.Contracts.Common;
using Garage.Domain.Bookings.Entities;
using MediatR;

namespace Garage.Application.Bookings.Queries.Search;

public sealed class SearchBookingsQueryHandler(IReadRepository<Booking> repo)
    : IRequestHandler<SearchBookingsQuery, QueryResult<BookingDto>>
{
    public async Task<QueryResult<BookingDto>> Handle(SearchBookingsQuery request, CancellationToken ct)
    {
        var search = request.Search;

        var query = repo.Query().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search.TextSearch))
        {
            var text = search.TextSearch.Trim();
            query = query.Where(b =>
                b.ClientNameAr.Contains(text) ||
                b.ClientNameEn.Contains(text) ||
                b.ClientPhone.Contains(text) ||
                b.ManufacturerNameAr.Contains(text) ||
                b.ManufacturerNameEn.Contains(text) ||
                b.CarMarkNameAr.Contains(text) ||
                b.CarMarkNameEn.Contains(text));
        }

        if (!string.IsNullOrWhiteSpace(search.Status)
            && Enum.TryParse<BookingStatus>(search.Status, ignoreCase: true, out var status))
        {
            query = query.Where(b => b.Status == status);
        }

        if (search.BranchId.HasValue)
            query = query.Where(b => b.BranchId == search.BranchId.Value);

        if (search.DateFrom.HasValue)
            query = query.Where(b => b.CreatedAtUtc >= search.DateFrom.Value);

        if (search.DateTo.HasValue)
            query = query.Where(b => b.CreatedAtUtc <= search.DateTo.Value);

        query = query.OrderByDescending(b => b.CreatedAtUtc);

        return await query
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
            .ToQueryResult(search.CurrentPage, search.ItemsPerPage);
    }
}
