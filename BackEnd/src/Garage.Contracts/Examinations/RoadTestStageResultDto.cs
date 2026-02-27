namespace Garage.Contracts.Examinations;

public sealed record RoadTestStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<RoadTestStageItemDto> Items);

public sealed record RoadTestStageItemDto(Guid IssueTypeId, Guid IssueId);
