using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Services.Enums;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetWorkflow;

public sealed class GetExaminationWorkflowQueryHandler(
    IReadRepository<Examination> repo,
    IApplicationDbContext db)
    : BaseQueryHandler<GetExaminationWorkflowQuery, ExaminationWorkflowDto?>
{
    public override async Task<ExaminationWorkflowDto?> Handle(
        GetExaminationWorkflowQuery request, CancellationToken ct)
    {
        // 1. Load the examination DTO
        var exam = await repo.Query()
            .Where(e => e.Id == request.Id)
            .Select(ExaminationProjection.ToDto)
            .FirstOrDefaultAsync(ct);

        if (exam is null) return null;

        // 2. Get the service IDs from the examination items
        var serviceIds = exam.Items.Select(i => i.ServiceId).Distinct().ToList();

        // 3. Get the stage mappings for these services
        var stageMappings = await db.ServicesStages
            .Where(ss => serviceIds.Contains(ss.ServiceId))
            .Select(ss => new { ss.ServiceId, ss.StageValue })
            .ToListAsync(ct);

        // 4. Build a lookup: serviceId → list of stage values
        var serviceStages = stageMappings
            .GroupBy(x => x.ServiceId)
            .ToDictionary(g => g.Key, g => g.Select(x => x.StageValue).ToList());

        // 5. Group items by stage, sorted by stage value
        var allStageValues = stageMappings.Select(x => x.StageValue).Distinct().OrderBy(v => v).ToList();

        var stages = allStageValues.Select(stageValue =>
        {
            var stage = Stage.FromValue(stageValue);
            var stageItems = exam.Items
                .Where(item => serviceStages.ContainsKey(item.ServiceId)
                            && serviceStages[item.ServiceId].Contains(stageValue))
                .ToList();

            return new WorkflowStageDto(
                stageValue,
                stage.Name,
                stage.NameAr,
                stageItems);
        }).ToList();

        return new ExaminationWorkflowDto(exam, stages);
    }
}
