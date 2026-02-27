using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Contracts.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetDashboardIndicatorsStage;

public sealed class GetDashboardIndicatorsStageHandler(IReadRepository<Examination> repo)
    : IRequestHandler<GetDashboardIndicatorsStageQuery, DashboardIndicatorsStageResultDto?>
{
    public async Task<DashboardIndicatorsStageResultDto?> Handle(GetDashboardIndicatorsStageQuery request, CancellationToken ct)
    {
        var result = await repo.Query()
            .Where(x => x.Id == request.ExaminationId)
            .Select(x => x.DashboardIndicatorsStageResult == null ? null : new DashboardIndicatorsStageResultDto(
                x.DashboardIndicatorsStageResult.Comments,
                x.DashboardIndicatorsStageResult.Items.Select(i =>
                    new DashboardIndicatorDto(i.Key, i.Value, i.NotApplicable)
                ).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}
