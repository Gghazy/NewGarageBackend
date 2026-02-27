using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using Garage.Domain.ExaminationManagement.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetSensorStage;

public sealed class GetSensorStageHandler(IReadRepository<SensorStageResult> repo)
    : IRequestHandler<GetSensorStageQuery, SensorStageResultDto?>
{
    public async Task<SensorStageResultDto?> Handle(GetSensorStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.ExaminationId == request.ExaminationId)
            .Select(x => new SensorStageResultDto(
                x.NoIssuesFound,
                x.CylinderCount,
                x.Comments,
                x.Items.Select(i =>
                    new SensorStageIssueDto(i.SensorIssueId, i.Evaluation)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
