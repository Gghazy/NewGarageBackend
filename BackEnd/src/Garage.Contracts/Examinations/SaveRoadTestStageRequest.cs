namespace Garage.Contracts.Examinations;

public sealed record SaveRoadTestStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<RoadTestStageItemRequest> Items);

public sealed record RoadTestStageItemRequest(Guid IssueTypeId, Guid IssueId);
