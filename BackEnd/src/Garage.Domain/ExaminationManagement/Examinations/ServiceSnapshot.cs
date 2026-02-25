using Garage.Domain.ExaminationManagement.Shared;


namespace Garage.Domain.ExaminationManagement.Examinations;
public sealed record ServiceSnapshot(
    Guid ServiceId,
    string NameAr,
    string NameEn,
    Money DefaultPrice
)
{
    // Parameterless constructor required by EF Core for owned types with navigation references
    private ServiceSnapshot() : this(Guid.Empty, string.Empty, string.Empty, null!) { }
}
