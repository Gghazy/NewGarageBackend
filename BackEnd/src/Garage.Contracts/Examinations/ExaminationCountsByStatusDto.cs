namespace Garage.Contracts.Examinations;

public sealed record ExaminationCountsByStatusDto(
    int Pending,
    int InProgress,
    int Completed
);
