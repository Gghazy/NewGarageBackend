using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetInteriorDecorStage;

public sealed class GetInteriorDecorStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetInteriorDecorStageQuery, InteriorDecorStageResultDto?>
{
    public async Task<InteriorDecorStageResultDto?> Handle(GetInteriorDecorStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.InteriorDecorStageResult == null ? null : new InteriorDecorStageResultDto(
                x.InteriorDecorStageResult.NoIssuesFound,
                x.InteriorDecorStageResult.Comments,
                x.InteriorDecorStageResult.Items.Select(i =>
                    new InteriorDecorItemDto(i.InsideAndDecorPartId, i.InsideAndDecorPartIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
