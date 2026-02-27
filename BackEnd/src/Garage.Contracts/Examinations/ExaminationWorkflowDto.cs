namespace Garage.Contracts.Examinations;

public sealed record ExaminationWorkflowDto(
    ExaminationDto Examination,
    List<WorkflowStageDto> Stages
);

public sealed record WorkflowStageDto(
    int Value,
    string NameEn,
    string NameAr,
    List<ExaminationItemDto> Items
);
