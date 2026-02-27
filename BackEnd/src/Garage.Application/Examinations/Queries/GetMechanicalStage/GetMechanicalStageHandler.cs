using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetMechanicalStage;

public sealed class GetMechanicalStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetMechanicalStageQuery, MechanicalStageResultDto?>
{
    public async Task<MechanicalStageResultDto?> Handle(GetMechanicalStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.MechanicalStageResult == null ? null : new MechanicalStageResultDto(
                x.MechanicalStageResult.NoIssuesFound,
                x.MechanicalStageResult.Comments,
                x.MechanicalStageResult.Items.Select(i =>
                    new MechanicalStageItemDto(i.MechIssueTypeId, i.MechIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
