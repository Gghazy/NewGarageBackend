using Cashif.Domain.Users.Permissions;
using Cashif.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Cashif.Infrastructure.Auth;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        var adminRole = await EnsureRole(roleMgr, "Admin");

        var admin = await userMgr.FindByEmailAsync("admin@cashif.local");
        if (admin is null)
        {
            admin = new AppUser { UserName = "admin@cashif.local", Email = "admin@cashif.local", NameEn = "Super Admin" };
            await userMgr.CreateAsync(admin, "Admin#12345");
            await userMgr.AddToRoleAsync(admin, adminRole.Name!);
        }
        await AddAdminPermissions(roleMgr,adminRole);
    }
    private static async Task<AppRole> EnsureRole(RoleManager<AppRole> roleMgr, string name)
    {
        var role = await roleMgr.FindByNameAsync(name);
        if (role is null) { role = new AppRole { Name = name }; await roleMgr.CreateAsync(role); }
        return role;
    }

    private static async Task AddAdminPermissions(RoleManager<AppRole> roleMgr,AppRole role)
    {
        var allClaims = await roleMgr.GetClaimsAsync(role);
        foreach (var p in Permission.All)
            if(!allClaims.Any(x=>x.Type== Permission.ClaimType && x.Value==p))
                  await roleMgr.AddClaimAsync(role, new Claim(Permission.ClaimType, p));
    }
}
