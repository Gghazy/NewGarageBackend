using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetRoadTestStage;

public sealed class GetRoadTestStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetRoadTestStageQuery, RoadTestStageResultDto?>
{
    public async Task<RoadTestStageResultDto?> Handle(GetRoadTestStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.RoadTestStageResult == null ? null : new RoadTestStageResultDto(
                x.RoadTestStageResult.NoIssuesFound,
                x.RoadTestStageResult.Comments,
                x.RoadTestStageResult.Items.Select(i =>
                    new RoadTestStageItemDto(i.RoadTestIssueTypeId, i.RoadTestIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
