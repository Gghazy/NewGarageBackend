namespace Garage.Contracts.Examinations;

public sealed record SaveAccessoryStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<AccessoryStageItemRequest> Items
);

public sealed record AccessoryStageItemRequest(Guid PartId, Guid IssueId);
