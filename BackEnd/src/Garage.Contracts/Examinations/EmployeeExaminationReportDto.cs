namespace Garage.Contracts.Examinations;

public sealed record EmployeeExaminationReportDto(
    Guid EmployeeId,
    Guid UserId,
    string NameAr,
    string NameEn,
    int ExaminationCount,
    int TotalStageActions,
    List<StageCountDto> StageCounts
);

public sealed record StageCountDto(
    string Stage,
    int Count
);
