namespace Garage.Contracts.Examinations;

public sealed record InteriorDecorStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<InteriorDecorItemDto> Items);

public sealed record InteriorDecorItemDto(Guid PartId, Guid IssueId);
