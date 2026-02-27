using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetInteriorBodyStage;

public sealed class GetInteriorBodyStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetInteriorBodyStageQuery, InteriorBodyStageResultDto?>
{
    public async Task<InteriorBodyStageResultDto?> Handle(GetInteriorBodyStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.InteriorBodyStageResult == null ? null : new InteriorBodyStageResultDto(
                x.InteriorBodyStageResult.NoIssuesFound,
                x.InteriorBodyStageResult.Comments,
                x.InteriorBodyStageResult.Items.Select(i =>
                    new InteriorBodyItemDto(i.InteriorBodyPartId, i.InteriorBodyIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
