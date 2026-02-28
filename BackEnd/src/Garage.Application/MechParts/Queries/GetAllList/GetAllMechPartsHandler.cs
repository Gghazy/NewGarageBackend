using Garage.Application.Abstractions;
using Garage.Contracts.MechParts;
using Garage.Domain.MechParts.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.MechParts.Queries.GetAllList;

public class GetAllMechPartsHandler(IReadRepository<MechPart> repo)
    : IRequestHandler<GetAllMechPartsQuery, List<MechPartResponse>>
{
    public async Task<List<MechPartResponse>> Handle(GetAllMechPartsQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Include(x => x.MechPartType)
            .Select(b => new MechPartResponse(
                b.Id,
                b.NameAr,
                b.NameEn,
                b.MechPartType.NameAr,
                b.MechPartType.NameEn,
                b.MechPartTypeId
            ))
            .ToListAsync(ct);
        return list;
    }
}
