using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using Garage.Domain.Branches.Entities;
using MediatR;
namespace Garage.Application.Branches.Queries.GetAllBranchesBySearch;
public class GetAllBranchesBySearchQueryHandler(IReadRepository<Branch> repo) : IRequestHandler<GetAllBranchesBySearchQuery, QueryResult<BranchDto>>
{
    public async Task<QueryResult<BranchDto>> Handle(GetAllBranchesBySearchQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Select(b => new BranchDto(b.Id, b.NameAr, b.NameEn, b.IsActive))
            .ToQueryResult(request.Search.CurrentPage,request.Search.ItemsPerPage);
        return list;
    }
}

