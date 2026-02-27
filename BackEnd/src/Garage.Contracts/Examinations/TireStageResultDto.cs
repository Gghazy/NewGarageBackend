namespace Garage.Contracts.Examinations;

public sealed record TireStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<TireStageItemDto> Items
);

public sealed record TireStageItemDto(string Position, int? Year, int? Week, string? Condition);
