using Garage.Domain.ExaminationManagement.Shared;


namespace Garage.Domain.ExaminationManagement.Examinations;
public sealed record ServiceSnapshot(
    Guid ServiceId,
    string NameAr,
    string NameEn,
    Money DefaultPrice
);
