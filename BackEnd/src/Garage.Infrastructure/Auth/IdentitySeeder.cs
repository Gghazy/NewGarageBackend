using Garage.Domain.Employees.Entities;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Auth.Entities;
using Garage.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Garage.Infrastructure.Auth;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider sp)
    {
        using var scope = sp.CreateScope();
        var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        // ── Admin role + user ──
        var adminRole = await EnsureRole(roleMgr, "Admin");

        var admin = await userMgr.FindByEmailAsync("admin@Garage.local");
        if (admin is null)
        {
            admin = new AppUser { UserName = "admin@Garage.local", Email = "admin@Garage.local" };
            await userMgr.CreateAsync(admin, "Admin#12345");
            await userMgr.AddToRoleAsync(admin, adminRole.Name!);
        }
        await EnsureAllPermissions(roleMgr, adminRole);

        // ── Manager role + user ──
        var managerRole = await EnsureRole(roleMgr, "Manager");
        await EnsureAllPermissions(roleMgr, managerRole);

        var managerEmail = "manager@garage.com";
        var manager = await userMgr.FindByEmailAsync(managerEmail);
        if (manager is null)
        {
            manager = new AppUser { UserName = managerEmail, Email = managerEmail };
            await userMgr.CreateAsync(manager, "123456");
            await userMgr.AddToRoleAsync(manager, managerRole.Name!);

            // Create Employee record
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var firstBranch = await db.Branches.FirstOrDefaultAsync();
            var branchIds = firstBranch is not null ? new List<Guid> { firstBranch.Id } : new List<Guid>();
            var employee = new Employee(manager.Id, "مدير النظام", "System Manager", branchIds);
            db.Employees.Add(employee);
            await db.SaveChangesAsync();
        }
    }

    private static async Task<AppRole> EnsureRole(RoleManager<AppRole> roleMgr, string name)
    {
        var role = await roleMgr.FindByNameAsync(name);
        if (role is null) { role = new AppRole { Name = name }; await roleMgr.CreateAsync(role); }
        return role;
    }

    private static async Task EnsureAllPermissions(RoleManager<AppRole> roleMgr, AppRole role)
    {
        var allClaims = await roleMgr.GetClaimsAsync(role);
        foreach (var p in Permission.All)
            if (!allClaims.Any(x => x.Type == Permission.ClaimType && x.Value == p))
                await roleMgr.AddClaimAsync(role, new Claim(Permission.ClaimType, p));
    }
}

