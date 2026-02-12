using Garage.Contracts.Auth;
using Garage.Contracts.Users;
using System.Security.Claims;
namespace Garage.Application.Abstractions;
public interface IIdentityService
{
    Task<(bool Succeeded, string? Error, Guid? UserId)> CreateUserAsync(RegisterUserRequest request, CancellationToken ct = default);
    Task<(bool Succeeded, Guid? UserId, string Email, IList<Claim>? claims)> ValidateUserAsync(LoginRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken ct = default);
}

