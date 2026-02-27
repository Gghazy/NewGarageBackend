namespace Garage.Contracts.Examinations;

public sealed record ExteriorBodyStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<ExteriorBodyItemDto> Items);

public sealed record ExteriorBodyItemDto(Guid PartId, Guid IssueId);
