namespace Garage.Contracts.Examinations;

public sealed record SensorStageResultDto(
    bool NoIssuesFound,
    int CylinderCount,
    string? Comments,
    List<SensorStageIssueDto> Issues
);

public sealed record SensorStageIssueDto(Guid IssueId, string Evaluation);
