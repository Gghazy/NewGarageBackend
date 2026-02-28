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

        var result = await query
            .Select(cm => new CarMarkDto(
                cm.Id,
                cm.NameAr,
                cm.NameEn,
                cm.ManufacturerId,
                cm.Manufacturer.NameAr,
                cm.Manufacturer.NameEn))
            .ToQueryResult(
                request.Search.CurrentPage,
                request.Search.ItemsPerPage,
                sort: request.Search.Sort,
                descending: request.Search.Desc,
                ct: ct);

        return result;
    }
}
