using Garage.Application.Abstractions;
using Garage.Application.Branches.Queries.GetAll;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.Services;
using Garage.Domain.Services.Entity;
using MediatR;


namespace Garage.Application.Services.Queries.GetAllPagination;
public sealed class GetAllServiceBySearchHandler(IReadRepository<Service> repo)
    : IRequestHandler<GetAllServiceBySearchQuery, QueryResult<ServiceDto>>
{
    public async Task<QueryResult<ServiceDto>> Handle(GetAllServiceBySearchQuery request, CancellationToken ct)
    {
        var query = repo.Query();

        if (!string.IsNullOrWhiteSpace(request.Search.TextSearch))
        {
            var term = request.Search.TextSearch.Trim();
            query = query.Where(x => x.NameAr.Contains(term) || x.NameEn.Contains(term));
        }

        var result = await query
            .Select(x => new ServiceDto(
                x.Id,
                x.NameAr,
                x.NameEn,
                new List<ServiceStageDto>() 
            ))
            .ToQueryResult(request.Search.CurrentPage, request.Search.ItemsPerPage);

        return result;
    }
}

