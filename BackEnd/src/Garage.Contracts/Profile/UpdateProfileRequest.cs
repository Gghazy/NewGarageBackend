namespace Garage.Contracts.Profile;

public sealed record UpdateProfileRequest(
    string NameAr,
    string NameEn,
    string Email,
    string? PhoneNumber);
