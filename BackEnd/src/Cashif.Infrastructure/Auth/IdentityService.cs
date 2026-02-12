using Cashif.Application.Abstractions;
using Cashif.Contracts.Auth;
using Cashif.Contracts.Users;
using Cashif.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Cashif.Infrastructure.Auth;

public class IdentityService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : IIdentityService
{
    public async Task<(bool Succeeded, string? Error, Guid? UserId)> CreateUserAsync(RegisterUserRequest request, CancellationToken ct = default)
    {
        var user = new AppUser { UserName = request.Email, Email = request.Email, PhoneNumber = request.Phone, NameAr = request.NameAr, NameEn = request.NameEn };
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
        var claims = await roleManager.GetClaimsAsync(role!);
        
        return (true, user.Id, user.Email!, claims);
    }

    public Task<IReadOnlyList<UserDto>> ListUsersAsync(CancellationToken ct = default)
    {
        var data = userManager.Users.Select(u => new UserDto(u.Id, u.NameAr, u.NameEn, u.Email!, u.PhoneNumber)).ToList();
        return Task.FromResult<IReadOnlyList<UserDto>>(data);
    }
}
