using Garage.Application.Abstractions;
using Garage.Contracts.SensorIssues;
using Garage.Domain.SensorIssues.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.SensorIssues.Queries.GetAllList;

public class GetAllSensorIssuesHandler(IReadRepository<SensorIssue> repo)
    : IRequestHandler<GetAllSensorIssuesQuery, List<SensorIssueDto>>
{
    public async Task<List<SensorIssueDto>> Handle(GetAllSensorIssuesQuery request, CancellationToken ct)
    {
        var list = await repo.Query()
            .Select(b => new SensorIssueDto(b.Id, b.NameAr, b.NameEn, b.Code))
            .ToListAsync(ct);
        return list;
    }
}
