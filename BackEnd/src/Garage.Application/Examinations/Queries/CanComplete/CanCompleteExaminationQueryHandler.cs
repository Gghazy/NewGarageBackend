using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.CanComplete;

public sealed class CanCompleteExaminationQueryHandler(
    IReadRepository<Examination> repo,
    IApplicationDbContext db)
    : BaseQueryHandler<CanCompleteExaminationQuery, bool>
{
    public override async Task<bool> Handle(CanCompleteExaminationQuery request, CancellationToken ct)
    {
        var exam = await repo.Query()
            .Include(e => e.Items)
            .Include(e => e.SensorStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.DashboardIndicatorsStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.InteriorBodyStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.ExteriorBodyStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.InteriorDecorStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.AccessoryStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.MechanicalStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.MechanicalStageResult!).ThenInclude(s => s.IssueItems)
            .Include(e => e.TireStageResult!).ThenInclude(s => s.Items)
            .Include(e => e.RoadTestStageResult!).ThenInclude(s => s.Items)
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (exam is null) return false;
        if (exam.Status != ExaminationStatus.InProgress) return false;

        // Get the service IDs from examination items
        var serviceIds = exam.Items.Select(i => i.Service.ServiceId).Distinct().ToList();

        // Get required stage values for these services
        var requiredStages = await db.ServicesStages
            .Where(ss => serviceIds.Contains(ss.ServiceId))
            .Select(ss => ss.StageValue)
            .Distinct()
            .ToListAsync(ct);

        if (requiredStages.Count == 0) return false;

        // Check each required stage has a saved result
        foreach (var stageValue in requiredStages)
        {
            if (!IsStageCompleted(exam, stageValue))
                return false;
        }

        return true;
    }

    private static bool IsStageCompleted(Examination exam, int stageValue) => stageValue switch
    {
        1 => exam.SensorStageResult is { } s1 && (s1.NoIssuesFound || s1.Items.Count > 0),
        2 => exam.DashboardIndicatorsStageResult is { } s2 && s2.Items.Count > 0,
        3 => exam.InteriorBodyStageResult is { } s3 && (s3.NoIssuesFound || s3.Items.Count > 0),
        4 => exam.ExteriorBodyStageResult is { } s4 && (s4.NoIssuesFound || s4.Items.Count > 0),
        5 => exam.InteriorDecorStageResult is { } s5 && (s5.NoIssuesFound || s5.Items.Count > 0),
        6 => exam.AccessoryStageResult is { } s6 && (s6.NoIssuesFound || s6.Items.Count > 0),
        7 => exam.MechanicalStageResult is { } s7 && (s7.NoIssuesFound || s7.Items.Count > 0 || s7.IssueItems.Count > 0),
        8 => exam.TireStageResult is { } s8 && (s8.NoIssuesFound || s8.Items.Count > 0),
        9 => exam.RoadTestStageResult is { } s9 && (s9.NoIssuesFound || s9.Items.Count > 0),
        _ => false,
    };
}
