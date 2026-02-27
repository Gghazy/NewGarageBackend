using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetTireStage;

public sealed class GetTireStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetTireStageQuery, TireStageResultDto?>
{
    public async Task<TireStageResultDto?> Handle(GetTireStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.TireStageResult == null ? null : new TireStageResultDto(
                x.TireStageResult.NoIssuesFound,
                x.TireStageResult.Comments,
                x.TireStageResult.Items.Select(i =>
                    new TireStageItemDto(i.Position, i.Year, i.Week, i.Condition)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
