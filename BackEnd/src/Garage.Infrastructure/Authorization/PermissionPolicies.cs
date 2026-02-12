using Garage.Domain.Users.Permissions;
using Microsoft.AspNetCore.Authorization;
namespace Garage.Infrastructure.Authorization;
public static class PermissionPolicies
{
    public static string PolicyName(string permission) => $"perm:{permission}";
    public static void AddPermissionPolicies(AuthorizationOptions options)
    {
        foreach (var p in Permission.All)
            options.AddPolicy(PolicyName(p), policy => policy.RequireClaim(Permission.ClaimType, p));
    }
}

