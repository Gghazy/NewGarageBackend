using Cashif.Contracts.Auth;
using Cashif.Contracts.Users;
using System.Security.Claims;
namespace Cashif.Application.Abstractions;
public interface IIdentityService
{
    Task<(bool Succeeded, string? Error, Guid? UserId)> CreateUserAsync(RegisterUserRequest request, CancellationToken ct = default);
    Task<(bool Succeeded, Guid? UserId, string Email, IList<Claim>? claims)> ValidateUserAsync(LoginRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken ct = default);
}
