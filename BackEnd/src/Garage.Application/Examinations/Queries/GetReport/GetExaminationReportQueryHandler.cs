using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Application.Examinations.Queries;
using Garage.Contracts.Examinations;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetReport;

public sealed class GetExaminationReportQueryHandler(
    IReadRepository<Examination> repo,
    IApplicationDbContext db)
    : BaseQueryHandler<GetExaminationReportQuery, ExaminationReportDto?>
{
    public override async Task<ExaminationReportDto?> Handle(
        GetExaminationReportQuery request, CancellationToken ct)
    {
        // 1. Load ExaminationDto via projection
        var examDto = await repo.Query()
            .Where(e => e.Id == request.Id)
            .Select(ExaminationProjection.ToDto)
            .FirstOrDefaultAsync(ct);

        if (examDto is null) return null;

        // 2. Load exam entity with all stage results + items
        var exam = await repo.Query()
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
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == request.Id, ct);

        if (exam is null) return null;

        // 3. Build each stage report with resolved names
        var sensorStage = await BuildSensorStage(exam, ct);
        var dashboardStage = BuildDashboardStage(exam);
        var interiorBodyStage = await BuildInteriorBodyStage(exam, ct);
        var exteriorBodyStage = await BuildExteriorBodyStage(exam, ct);
        var interiorDecorStage = await BuildInteriorDecorStage(exam, ct);
        var accessoryStage = await BuildAccessoryStage(exam, ct);
        var mechanicalStage = await BuildMechanicalStage(exam, ct);
        var tireStage = BuildTireStage(exam);
        var roadTestStage = await BuildRoadTestStage(exam, ct);

        return new ExaminationReportDto(
            examDto,
            sensorStage,
            dashboardStage,
            interiorBodyStage,
            exteriorBodyStage,
            interiorDecorStage,
            accessoryStage,
            mechanicalStage,
            tireStage,
            roadTestStage);
    }

    // ── Stage 1: Sensors ─────────────────────────────────────────────────────
    private async Task<SensorStageReportDto?> BuildSensorStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.SensorStageResult;
        if (stage is null) return null;

        var issueIds = stage.Items.Select(i => i.SensorIssueId).Distinct().ToList();
        var lookup = issueIds.Count > 0
            ? await db.SensorIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var issues = stage.Items.Select(i =>
        {
            lookup.TryGetValue(i.SensorIssueId, out var si);
            return new SensorReportIssueDto(
                si?.NameAr ?? "", si?.NameEn ?? "", si?.Code, i.Evaluation);
        }).ToList();

        return new SensorStageReportDto(stage.NoIssuesFound, stage.CylinderCount, stage.Comments, issues);
    }

    // ── Stage 2: Dashboard Indicators ────────────────────────────────────────
    private static DashboardStageReportDto? BuildDashboardStage(Examination exam)
    {
        var stage = exam.DashboardIndicatorsStageResult;
        if (stage is null) return null;

        var indicators = stage.Items.Select(i =>
            new DashboardReportIndicatorDto(i.Key, i.Value, i.NotApplicable)).ToList();

        return new DashboardStageReportDto(stage.Comments, indicators);
    }

    // ── Stage 3: Interior Body ───────────────────────────────────────────────
    private async Task<PartIssueStageReportDto?> BuildInteriorBodyStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.InteriorBodyStageResult;
        if (stage is null) return null;

        var partIds = stage.Items.Select(i => i.InteriorBodyPartId).Distinct().ToList();
        var issueIds = stage.Items.Select(i => i.InteriorBodyIssueId).Distinct().ToList();

        var parts = partIds.Count > 0
            ? await db.InteriorBodyParts.Where(x => partIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var issues = issueIds.Count > 0
            ? await db.InteriorBodyIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var items = stage.Items.Select(i =>
        {
            parts.TryGetValue(i.InteriorBodyPartId, out var p);
            issues.TryGetValue(i.InteriorBodyIssueId, out var iss);
            return new PartIssueReportItemDto(
                p?.NameAr ?? "", p?.NameEn ?? "",
                iss?.NameAr ?? "", iss?.NameEn ?? "");
        }).ToList();

        return new PartIssueStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }

    // ── Stage 4: Exterior Body ───────────────────────────────────────────────
    private async Task<PartIssueStageReportDto?> BuildExteriorBodyStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.ExteriorBodyStageResult;
        if (stage is null) return null;

        var partIds = stage.Items.Select(i => i.ExteriorBodyPartId).Distinct().ToList();
        var issueIds = stage.Items.Select(i => i.ExteriorBodyIssueId).Distinct().ToList();

        var parts = partIds.Count > 0
            ? await db.ExteriorBodyParts.Where(x => partIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var issues = issueIds.Count > 0
            ? await db.ExteriorBodyIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var items = stage.Items.Select(i =>
        {
            parts.TryGetValue(i.ExteriorBodyPartId, out var p);
            issues.TryGetValue(i.ExteriorBodyIssueId, out var iss);
            return new PartIssueReportItemDto(
                p?.NameAr ?? "", p?.NameEn ?? "",
                iss?.NameAr ?? "", iss?.NameEn ?? "");
        }).ToList();

        return new PartIssueStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }

    // ── Stage 5: Interior Decor ──────────────────────────────────────────────
    private async Task<PartIssueStageReportDto?> BuildInteriorDecorStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.InteriorDecorStageResult;
        if (stage is null) return null;

        var partIds = stage.Items.Select(i => i.InsideAndDecorPartId).Distinct().ToList();
        var issueIds = stage.Items.Select(i => i.InsideAndDecorPartIssueId).Distinct().ToList();

        var parts = partIds.Count > 0
            ? await db.InsideAndDecorParts.Where(x => partIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var issues = issueIds.Count > 0
            ? await db.InsideAndDecorPartIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var items = stage.Items.Select(i =>
        {
            parts.TryGetValue(i.InsideAndDecorPartId, out var p);
            issues.TryGetValue(i.InsideAndDecorPartIssueId, out var iss);
            return new PartIssueReportItemDto(
                p?.NameAr ?? "", p?.NameEn ?? "",
                iss?.NameAr ?? "", iss?.NameEn ?? "");
        }).ToList();

        return new PartIssueStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }

    // ── Stage 6: Accessories ─────────────────────────────────────────────────
    private async Task<PartIssueStageReportDto?> BuildAccessoryStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.AccessoryStageResult;
        if (stage is null) return null;

        var partIds = stage.Items.Select(i => i.AccessoryPartId).Distinct().ToList();
        var issueIds = stage.Items.Select(i => i.AccessoryIssueId).Distinct().ToList();

        var parts = partIds.Count > 0
            ? await db.AccessoryParts.Where(x => partIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var issues = issueIds.Count > 0
            ? await db.AccessoryIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var items = stage.Items.Select(i =>
        {
            parts.TryGetValue(i.AccessoryPartId, out var p);
            issues.TryGetValue(i.AccessoryIssueId, out var iss);
            return new PartIssueReportItemDto(
                p?.NameAr ?? "", p?.NameEn ?? "",
                iss?.NameAr ?? "", iss?.NameEn ?? "");
        }).ToList();

        return new PartIssueStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }

    // ── Stage 7: Mechanical ──────────────────────────────────────────────────
    private async Task<MechanicalStageReportDto?> BuildMechanicalStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.MechanicalStageResult;
        if (stage is null) return null;

        var partTypeIds = stage.Items.Select(i => i.MechPartTypeId).Distinct().ToList();
        var partIds = stage.Items.Select(i => i.MechPartId).Distinct().ToList();
        var issuePartIds = stage.IssueItems.Select(i => i.MechPartId).Distinct().ToList();
        var issueIds = stage.IssueItems.Select(i => i.MechIssueId).Distinct().ToList();

        var allPartIds = partIds.Union(issuePartIds).Distinct().ToList();

        var partTypes = partTypeIds.Count > 0
            ? await db.MechPartTypes.Where(x => partTypeIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var mechParts = allPartIds.Count > 0
            ? await db.MechParts.Where(x => allPartIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var mechIssues = issueIds.Count > 0
            ? await db.MechIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        // Group issues by MechPartId
        var issuesByPart = stage.IssueItems
            .GroupBy(i => i.MechPartId)
            .ToDictionary(g => g.Key, g => g.Select(i =>
            {
                mechIssues.TryGetValue(i.MechIssueId, out var mi);
                return new MechReportIssueDto(mi?.NameAr ?? "", mi?.NameEn ?? "");
            }).ToList());

        var rows = stage.Items.Select(i =>
        {
            partTypes.TryGetValue(i.MechPartTypeId, out var pt);
            mechParts.TryGetValue(i.MechPartId, out var mp);
            issuesByPart.TryGetValue(i.MechPartId, out var partIssues);
            return new MechReportRowDto(
                pt?.NameAr ?? "", pt?.NameEn ?? "",
                mp?.NameAr ?? "", mp?.NameEn ?? "",
                partIssues ?? []);
        }).ToList();

        return new MechanicalStageReportDto(stage.NoIssuesFound, stage.Comments, rows);
    }

    // ── Stage 8: Tires ───────────────────────────────────────────────────────
    private static TireStageReportDto? BuildTireStage(Examination exam)
    {
        var stage = exam.TireStageResult;
        if (stage is null) return null;

        var items = stage.Items.Select(i =>
            new TireReportItemDto(i.Position, i.Year, i.Week, i.Condition)).ToList();

        return new TireStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }

    // ── Stage 9: Road Test ───────────────────────────────────────────────────
    private async Task<RoadTestStageReportDto?> BuildRoadTestStage(Examination exam, CancellationToken ct)
    {
        var stage = exam.RoadTestStageResult;
        if (stage is null) return null;

        var typeIds = stage.Items.Select(i => i.RoadTestIssueTypeId).Distinct().ToList();
        var issueIds = stage.Items.Select(i => i.RoadTestIssueId).Distinct().ToList();

        var types = typeIds.Count > 0
            ? await db.RoadTestIssueTypes.Where(x => typeIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];
        var issues = issueIds.Count > 0
            ? await db.RoadTestIssues.Where(x => issueIds.Contains(x.Id)).ToDictionaryAsync(x => x.Id, ct)
            : [];

        var items = stage.Items.Select(i =>
        {
            types.TryGetValue(i.RoadTestIssueTypeId, out var t);
            issues.TryGetValue(i.RoadTestIssueId, out var iss);
            return new RoadTestReportItemDto(
                t?.NameAr ?? "", t?.NameEn ?? "",
                iss?.NameAr ?? "", iss?.NameEn ?? "");
        }).ToList();

        return new RoadTestStageReportDto(stage.NoIssuesFound, stage.Comments, items);
    }
}
