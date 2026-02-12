using Cashif.Application.Abstractions;
using Cashif.Application.Common.Extensions;
using Cashif.Contracts.Branches;
using Cashif.Contracts.Common;
using Cashif.Domain.Branches.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Cashif.Application.Branches.Queries.GetAll;
public class GetBranchHandler(IReadRepository<Branch> repo) : IRequestHandler<GetBranchQuery, QueryResult<BranchDto>>
{
    public async Task<QueryResult<BranchDto>> Handle(GetBranchQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Select(b => new BranchDto(b.Id, b.NameAr, b.NameEn, b.IsActive))
            .ToQueryResult(request.Search.CurrentPage,request.Search.ItemsPerPage);
        return list;
    }
}
