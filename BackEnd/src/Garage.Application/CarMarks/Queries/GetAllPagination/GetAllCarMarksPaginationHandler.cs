using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.CarMarks;
using Garage.Contracts.Common;
using Garage.Domain.CarMarkes.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.CarMarks.Queries.GetAllPagination;

public sealed class GetAllCarMarksPaginationHandler(IReadRepository<CarMark> repo)
    : IRequestHandler<GetAllCarMarksPaginationQuery, QueryResult<CarMarkDto>>
{
    public async Task<QueryResult<CarMarkDto>> Handle(GetAllCarMarksPaginationQuery request, CancellationToken ct)
    {
        var query = repo.Query().Include(cm => cm.Manufacturer).AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search.TextSearch))
        {
            var term = request.Search.TextSearch.Trim();
            query = query.Where(cm =>
                cm.NameAr.Contains(term) ||
                cm.NameEn.Contains(term) ||
                cm.Manufacturer.NameAr.Contains(term) ||
                cm.Manufacturer.NameEn.Contains(term));
        }

        // Sort on entity before projecting (EF Core can't translate sorting on constructed DTOs)
        var sort = request.Search.Sort?.Trim().ToLowerInvariant();
        query = sort switch
        {
            "manufacturernamear" => request.Search.Desc ? query.OrderByDescending(cm => cm.Manufacturer.NameAr) : query.OrderBy(cm => cm.Manufacturer.NameAr),
            "manufacturernameen" => request.Search.Desc ? query.OrderByDescending(cm => cm.Manufacturer.NameEn) : query.OrderBy(cm => cm.Manufacturer.NameEn),
            "nameen" => request.Search.Desc ? query.OrderByDescending(cm => cm.NameEn) : query.OrderBy(cm => cm.NameEn),
            _ => request.Search.Desc ? query.OrderByDescending(cm => cm.NameAr) : query.OrderBy(cm => cm.NameAr),
        };

        var count = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Search.CurrentPage - 1) * request.Search.ItemsPerPage)
            .Take(request.Search.ItemsPerPage)
            .Select(cm => new CarMarkDto(
                cm.Id,
                cm.NameAr,
                cm.NameEn,
                cm.ManufacturerId,
                cm.Manufacturer.NameAr,
                cm.Manufacturer.NameEn))
            .ToListAsync(ct);

        return new QueryResult<CarMarkDto>(items: items, totalCount: count, pageNumber: request.Search.CurrentPage, pageSize: request.Search.ItemsPerPage);
    }
}
