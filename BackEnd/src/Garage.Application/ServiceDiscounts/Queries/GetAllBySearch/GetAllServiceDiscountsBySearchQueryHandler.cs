using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.ServiceDiscounts;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServiceDiscounts.Queries.GetAllBySearch;

public class GetAllServiceDiscountsBySearchQueryHandler(IApplicationDbContext _dbContext)
    : IRequestHandler<GetAllServiceDiscountsBySearchQuery, QueryResult<ServiceDiscountDto>>
{
    public async Task<QueryResult<ServiceDiscountDto>> Handle(GetAllServiceDiscountsBySearchQuery command, CancellationToken ct)
    {
        var filter = command.Request;

        var discounts = _dbContext.ServiceDiscounts.AsNoTracking().AsQueryable();

        if (filter.ServiceId.HasValue)
            discounts = discounts.Where(d => d.ServiceId == filter.ServiceId.Value);

        if (filter.IsActive.HasValue)
            discounts = discounts.Where(d => d.IsActive == filter.IsActive.Value);

        var query = from d in discounts
                    join s in _dbContext.Services.AsNoTracking()
                        on d.ServiceId equals s.Id
                    orderby d.CreatedAtUtc descending
                    select new ServiceDiscountDto(
                        d.Id,
                        d.ServiceId,
                        s.NameAr,
                        s.NameEn,
                        d.DiscountPercent,
                        d.StartDate,
                        d.EndDate,
                        d.IsActive
                    );

        return await query.ToQueryResult(filter.Search.CurrentPage, filter.Search.ItemsPerPage, ct: ct);
    }
}
