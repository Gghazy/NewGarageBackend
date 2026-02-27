namespace Garage.Contracts.Examinations;

public sealed record SaveInteriorDecorStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<InteriorDecorItemRequest> Items);

public sealed record InteriorDecorItemRequest(Guid PartId, Guid IssueId);
