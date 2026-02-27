using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetSensorStage;

public sealed class GetSensorStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetSensorStageQuery, SensorStageResultDto?>
{
    public async Task<SensorStageResultDto?> Handle(GetSensorStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.SensorStageResult == null ? null : new SensorStageResultDto(
                x.SensorStageResult.NoIssuesFound,
                x.SensorStageResult.CylinderCount,
                x.SensorStageResult.Comments,
                x.SensorStageResult.Items.Select(i =>
                    new SensorStageIssueDto(i.SensorIssueId, i.Evaluation)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
