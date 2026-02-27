namespace Garage.Contracts.Examinations;

public sealed record InteriorBodyStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<InteriorBodyItemDto> Items);

public sealed record InteriorBodyItemDto(Guid PartId, Guid IssueId);
