using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.SensorIssues;
using Garage.Domain.SensorIssues.Entities;
using MediatR;

namespace Garage.Application.SensorIssues.Queries.GetAll;
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

