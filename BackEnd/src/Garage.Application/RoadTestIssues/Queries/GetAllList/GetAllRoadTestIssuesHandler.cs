using Garage.Application.Abstractions;
using Garage.Contracts.RoadTestIssues;
using Garage.Domain.RoadTestIssues.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.RoadTestIssues.Queries.GetAllList;

public class GetAllRoadTestIssuesHandler(IReadRepository<RoadTestIssue> repo)
    : IRequestHandler<GetAllRoadTestIssuesQuery, List<RoadTestIssueResponse>>
{
    public async Task<List<RoadTestIssueResponse>> Handle(GetAllRoadTestIssuesQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Include(x => x.RoadTestIssueType)
            .Select(b => new RoadTestIssueResponse(
                b.Id,
                b.NameAr,
                b.NameEn,
                b.RoadTestIssueType.NameAr,
                b.RoadTestIssueType.NameEn,
                b.RoadTestIssueTypeId
            ))
            .ToListAsync(ct);
        return list;
    }
}
