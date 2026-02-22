

namespace Garage.Domain.ExaminationManagement.Examinations;

public sealed record BranchReference(
 Guid BranchId,
 string NameAr,
 string NameEn
);
