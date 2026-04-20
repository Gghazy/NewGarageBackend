using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.ServicePointRules;
using Garage.Domain.ServicePointRules.Entities;
using MediatR;

namespace Garage.Application.ServicePointRules.Queries.GetAllBySearch;

public class GetAllServicePointRulesBySearchQueryHandler(IReadRepository<ServicePointRule> repo)
    : IRequestHandler<GetAllServicePointRulesBySearchQuery, QueryResult<ServicePointRuleDto>>
{
    public async Task<QueryResult<ServicePointRuleDto>> Handle(GetAllServicePointRulesBySearchQuery command, CancellationToken ct)
    {
        var filter = command.Request;
        var rules = repo.Query().AsQueryable();

        if (filter.IsActive.HasValue)
            rules = rules.Where(r => r.IsActive == filter.IsActive.Value);

        var query = rules
            .OrderBy(r => r.FromAmount)
            .Select(r => new ServicePointRuleDto(r.Id, r.FromAmount, r.ToAmount, r.Points, r.IsActive));

        return await query.ToQueryResult(filter.Search.CurrentPage, filter.Search.ItemsPerPage, ct: ct);
    }
}
