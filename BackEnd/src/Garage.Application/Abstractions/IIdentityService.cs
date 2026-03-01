using Garage.Contracts.Auth;
using Garage.Contracts.Users;
using System.Security.Claims;
namespace Garage.Application.Abstractions;
public interface IIdentityService
{
    Task<(bool Succeeded, string? Error, Guid? UserId)> CreateUserAsync(RegisterUserRequest request, CancellationToken ct = default);
    Task<(bool Succeeded, Guid? UserId, string Email, IList<Claim>? claims)> ValidateUserAsync(LoginRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken ct = default);
    Task LockUserAsync(Guid userId, CancellationToken ct = default);
    Task<(bool Succeeded, string? Error)> UpdateEmailAndPhoneAsync(Guid userId, string email, string phoneNumber, CancellationToken ct = default);
    Task<(bool Succeeded, string? Error)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);
    Task<IList<Claim>?> GetUserClaimsAsync(Guid userId, CancellationToken ct = default);
}

