namespace Garage.Contracts.Examinations;

public sealed record SaveMechanicalStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<MechanicalStageItemRequest> Items,
    List<MechanicalStageIssueItemRequest> IssueItems);

public sealed record MechanicalStageItemRequest(Guid PartTypeId, Guid PartId);

public sealed record MechanicalStageIssueItemRequest(Guid PartId, Guid IssueId);
