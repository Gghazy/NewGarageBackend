namespace Garage.Contracts.Examinations;

public sealed record SaveMechanicalStageRequest(
    bool NoIssuesFound,
    string? Comments,
    List<MechanicalStageItemRequest> Items);

public sealed record MechanicalStageItemRequest(Guid PartTypeId, Guid PartId);
