namespace Garage.Contracts.Examinations;

public sealed record SaveExteriorBodyStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<ExteriorBodyItemRequest> Items);

public sealed record ExteriorBodyItemRequest(Guid PartId, Guid IssueId);
