namespace Garage.Contracts.Examinations;

public sealed record MechanicalStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<MechanicalStageItemDto> Items,
    List<MechanicalStageIssueItemDto> IssueItems);

public sealed record MechanicalStageItemDto(Guid PartTypeId, Guid PartId);

public sealed record MechanicalStageIssueItemDto(Guid PartId, Guid IssueId);
