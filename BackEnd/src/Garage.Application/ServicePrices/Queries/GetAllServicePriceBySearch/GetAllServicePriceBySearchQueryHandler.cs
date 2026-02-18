using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Common.Extensions;
using Garage.Application.SensorIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Contracts.ServicePrices;
using Garage.Domain.ServicePrices.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServicePrices.Queries.GetAllServicePriceBySearch;

public class GetAllServicePriceBySearchQueryHandler(IApplicationDbContext _dbContext) : IRequestHandler<GetAllServicePriceBySearchQuery, QueryResult<ServicePriceDto>>
{
    public async Task<QueryResult<ServicePriceDto>> Handle(GetAllServicePriceBySearchQuery command, CancellationToken ct)
    {

        var query = from sp in _dbContext.ServicePrices.AsNoTracking()

                    join s in _dbContext.Services.AsNoTracking()
                        on sp.ServiceId equals s.Id

                    join m in _dbContext.CarMarks.AsNoTracking()
                        on sp.MarkId equals m.Id

                    select new ServicePriceDto
                    (
                        sp.Id,
                        sp.ServiceId,
                        s.NameAr,
                        s.NameEn,
                        sp.MarkId,
                        m.NameAr,
                        m.NameEn,
                        sp.FromYear,
                        sp.ToYear,
                        sp.Price
                    );


        return await query.ToQueryResult(command.Request.Search.CurrentPage, command.Request.Search.ItemsPerPage, ct: ct);
    }
}
