namespace Garage.Contracts.Examinations;

public sealed record AccessoryStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<AccessoryStageItemDto> Items
);

public sealed record AccessoryStageItemDto(Guid PartId, Guid IssueId);
