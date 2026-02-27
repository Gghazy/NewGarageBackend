namespace Garage.Contracts.Examinations;

public sealed record MechanicalStageResultDto(
    bool NoIssuesFound,
    string? Comments,
    List<MechanicalStageItemDto> Items);

public sealed record MechanicalStageItemDto(Guid PartTypeId, Guid PartId);
