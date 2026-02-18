using Garage.Application.Abstractions;
using Garage.Application.Branches.Queries.GetAllBranchesBySearch;
using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using Garage.Domain.Branches.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Branches.Queries.GetAll;


public class GetAllBranchesQueryHandler(IReadRepository<Branch> repo) : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
{
    public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken ct)
    {
        var list = await repo.Query().Select(b => new BranchDto(b.Id, b.NameAr, b.NameEn, b.IsActive)).ToListAsync();
        return list;
    }
}


