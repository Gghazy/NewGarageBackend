namespace Garage.Contracts.Examinations;

public sealed record SaveSensorStageRequest(
    bool NoIssuesFound,
    int CylinderCount,
    string? Comments,
    List<SensorStageIssueRequest> Issues
);

public sealed record SensorStageIssueRequest(Guid IssueId, string Evaluation);
