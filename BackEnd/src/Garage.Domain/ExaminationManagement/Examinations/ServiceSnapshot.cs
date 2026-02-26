namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed record ServiceSnapshot(
    Guid ServiceId,
    string NameAr,
    string NameEn
)
{
    private ServiceSnapshot() : this(Guid.Empty, string.Empty, string.Empty) { }
}
