namespace Garage.Contracts.Profile;

public sealed record ProfileDto(
    string NameAr,
    string NameEn,
    string Email,
    string? PhoneNumber,
    string RoleName,
    List<ProfileBranchDto> Branches);

public sealed record ProfileBranchDto(Guid BranchId, string BranchNameAr, string BranchNameEn);
