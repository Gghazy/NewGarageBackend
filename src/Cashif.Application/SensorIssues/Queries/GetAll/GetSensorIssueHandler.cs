using Cashif.Application.Abstractions;
using Cashif.Application.Common.Extensions;
using Cashif.Contracts.Common;
using Cashif.Contracts.SensorIssues;
using Cashif.Domain.SensorIssues.Entities;
using MediatR;

namespace Cashif.Application.SensorIssues.Queries.GetAll;
public class GetSensorIssueHandler(IReadRepository<SensorIssue> repo) : IRequestHandler<GetSensorIssueQuery, QueryResult<SensorIssueDto>>
{
    public async Task<QueryResult<SensorIssueDto>> Handle(GetSensorIssueQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Select(b => new SensorIssueDto(b.Id, b.NameAr, b.NameEn, b.Code))
            .ToQueryResult(request.Search.CurrentPage,request.Search.ItemsPerPage);
        return list;
    }
}
