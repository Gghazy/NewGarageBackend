using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.RoadTestIssues;
using Garage.Domain.RoadTestIssues.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.RoadTestIssues.Queries.GetAll;

public class GetRoadTestIssueHandler(IReadRepository<RoadTestIssue> repo)
    : IRequestHandler<GetRoadTestIssueQuery, QueryResult<RoadTestIssueResponse>>
{
    public async Task<QueryResult<RoadTestIssueResponse>> Handle(GetRoadTestIssueQuery request, CancellationToken ct)
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
            )).ToQueryResult(request.Search.CurrentPage, request.Search.ItemsPerPage);
        return list;
    }
}
