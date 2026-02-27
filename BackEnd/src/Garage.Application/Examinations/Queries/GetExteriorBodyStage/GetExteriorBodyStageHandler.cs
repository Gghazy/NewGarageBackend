using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetExteriorBodyStage;

public sealed class GetExteriorBodyStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetExteriorBodyStageQuery, ExteriorBodyStageResultDto?>
{
    public async Task<ExteriorBodyStageResultDto?> Handle(GetExteriorBodyStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.ExteriorBodyStageResult == null ? null : new ExteriorBodyStageResultDto(
                x.ExteriorBodyStageResult.NoIssuesFound,
                x.ExteriorBodyStageResult.Comments,
                x.ExteriorBodyStageResult.Items.Select(i =>
                    new ExteriorBodyItemDto(i.ExteriorBodyPartId, i.ExteriorBodyIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
