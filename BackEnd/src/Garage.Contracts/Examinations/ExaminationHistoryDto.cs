namespace Garage.Contracts.Examinations;

public sealed record ExaminationHistoryDto(
    Guid Id,
    string Action,
    string? Details,
    Guid? PerformedById,
    string? PerformedByNameAr,
    string? PerformedByNameEn,
    DateTime CreatedAtUtc
);
