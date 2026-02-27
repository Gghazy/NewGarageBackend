namespace Garage.Contracts.Examinations;

public sealed record SaveTireStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<TireStageItemRequest> Items
);

public sealed record TireStageItemRequest(string Position, int? Year, int? Week, string? Condition);
