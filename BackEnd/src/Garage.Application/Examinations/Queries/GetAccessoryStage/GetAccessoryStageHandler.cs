using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetAccessoryStage;

public sealed class GetAccessoryStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetAccessoryStageQuery, AccessoryStageResultDto?>
{
    public async Task<AccessoryStageResultDto?> Handle(GetAccessoryStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.AccessoryStageResult == null ? null : new AccessoryStageResultDto(
                x.AccessoryStageResult.NoIssuesFound,
                x.AccessoryStageResult.Comments,
                x.AccessoryStageResult.Items.Select(i =>
                    new AccessoryStageItemDto(i.AccessoryPartId, i.AccessoryIssueId)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
