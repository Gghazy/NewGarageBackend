namespace Garage.Contracts.Examinations;

// ── Wrapper ──────────────────────────────────────────────────────────────────
public sealed record ExaminationReportDto(
    ExaminationDto Examination,
    SensorStageReportDto? SensorStage,
    DashboardStageReportDto? DashboardIndicatorsStage,
    PartIssueStageReportDto? InteriorBodyStage,
    PartIssueStageReportDto? ExteriorBodyStage,
    PartIssueStageReportDto? InteriorDecorStage,
    PartIssueStageReportDto? AccessoryStage,
    MechanicalStageReportDto? MechanicalStage,
    TireStageReportDto? TireStage,
    RoadTestStageReportDto? RoadTestStage
);

// ── Stage 1: Sensors ─────────────────────────────────────────────────────────
public sealed record SensorStageReportDto(
    bool NoIssuesFound, int CylinderCount, string? Comments,
    List<SensorReportIssueDto> Issues);

public sealed record SensorReportIssueDto(
    string NameAr, string NameEn, string? Code, string Evaluation);

// ── Stage 2: Dashboard Indicators ────────────────────────────────────────────
public sealed record DashboardStageReportDto(
    string? Comments,
    List<DashboardReportIndicatorDto> Indicators);

public sealed record DashboardReportIndicatorDto(
    string Key, decimal? Value, bool NotApplicable);

// ── Stages 3,4,5,6: Part + Issue (shared shape) ─────────────────────────────
public sealed record PartIssueStageReportDto(
    bool NoIssuesFound, string? Comments,
    List<PartIssueReportItemDto> Items);

public sealed record PartIssueReportItemDto(
    string PartNameAr, string PartNameEn,
    string IssueNameAr, string IssueNameEn);

// ── Stage 7: Mechanical ──────────────────────────────────────────────────────
public sealed record MechanicalStageReportDto(
    bool NoIssuesFound, string? Comments,
    List<MechReportRowDto> Rows);

public sealed record MechReportRowDto(
    string PartTypeNameAr, string PartTypeNameEn,
    string PartNameAr, string PartNameEn,
    List<MechReportIssueDto> Issues);

public sealed record MechReportIssueDto(string NameAr, string NameEn);

// ── Stage 8: Tires ───────────────────────────────────────────────────────────
public sealed record TireStageReportDto(
    bool NoIssuesFound, string? Comments,
    List<TireReportItemDto> Items);

public sealed record TireReportItemDto(
    string Position, int? Year, int? Week, string? Condition);

// ── Stage 9: Road Test ───────────────────────────────────────────────────────
public sealed record RoadTestStageReportDto(
    bool NoIssuesFound, string? Comments,
    List<RoadTestReportItemDto> Items);

public sealed record RoadTestReportItemDto(
    string IssueTypeNameAr, string IssueTypeNameEn,
    string IssueNameAr, string IssueNameEn);
