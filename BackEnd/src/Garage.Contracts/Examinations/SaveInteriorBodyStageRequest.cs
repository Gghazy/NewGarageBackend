namespace Garage.Contracts.Examinations;

public sealed record SaveInteriorBodyStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<InteriorBodyItemRequest> Items);

public sealed record InteriorBodyItemRequest(Guid PartId, Guid IssueId);
