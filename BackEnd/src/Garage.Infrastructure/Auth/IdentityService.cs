using Garage.Application.Abstractions;
using Garage.Contracts.Auth;
using Garage.Contracts.Users;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Garage.Infrastructure.Auth;

public class IdentityService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : IIdentityService
{
    public async Task<(bool Succeeded, string? Error, Guid? UserId)> CreateUserAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        var user = new AppUser { UserName = request.Email, Email = request.Email, PhoneNumber = request.Phone};
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return (false, string.Join(";", result.Errors.Select(e => e.Description)), null);
        return (true, null, user.Id);
    }

    public async Task<(bool Succeeded, Guid? UserId, string Email,IList<Claim>? claims)> ValidateUserAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null) return (false, null, string.Empty,null);
        var ok = await userManager.CheckPasswordAsync(user, request.Password);
        if (!ok) return (false, null, string.Empty, null);
        var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
        var role = await roleManager.FindByNameAsync(userRole);
        if (role is null) return (true, user.Id, user.Email!, null);
        var claims = await roleManager.GetClaimsAsync(role);

        return (true, user.Id, user.Email!, claims);
    }

    public Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken ct = default)
    {
        var data = userManager.Users.Select(u => new UserDto(u.Id, u.Email!, u.PhoneNumber)).ToList();
        return Task.FromResult<IReadOnlyList<UserDto>>(data);
    }

    public async Task LockUserAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return;
        await userManager.SetLockoutEnabledAsync(user, true);
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
    }

    public async Task<(bool Succeeded, string? Error)> UpdateEmailAndPhoneAsync(
        Guid userId, string email, string phoneNumber, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return (false, "User not found");

        if (!string.Equals(user.Email, email, StringComparison.OrdinalIgnoreCase))
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null && existing.Id != userId)
                return (false, "Email is already in use");

            user.Email = email;
            user.NormalizedEmail = email.ToUpperInvariant();
            user.UserName = email;
            user.NormalizedUserName = email.ToUpperInvariant();
        }

        user.PhoneNumber = phoneNumber;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    public async Task<(bool Succeeded, string? Error)> ChangePasswordAsync(
        Guid userId, string currentPassword, string newPassword, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return (false, "User not found");

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        if (!result.Succeeded)
            return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

        return (true, null);
    }

    public async Task<IList<Claim>?> GetUserClaimsAsync(Guid userId, CancellationToken ct = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null) return null;

        var userRole = (await userManager.GetRolesAsync(user)).FirstOrDefault();
        if (userRole is null) return null;

        var role = await roleManager.FindByNameAsync(userRole);
        if (role is null) return null;

        return await roleManager.GetClaimsAsync(role);
    }
}

