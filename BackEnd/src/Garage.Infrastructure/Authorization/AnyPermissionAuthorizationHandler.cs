using Garage.Domain.Users.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Garage.Infrastructure.Authorization;

public class AnyPermissionAuthorizationHandler : AuthorizationHandler<AnyPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context, AnyPermissionRequirement requirement)
    {
        if (context.User?.Identity?.IsAuthenticated != true)
            return Task.CompletedTask;

        var userPermissions = context.User
            .FindAll(Permission.ClaimType)
            .Select(c => c.Value);

        if (requirement.Permissions.Any(p =>
                userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase)))
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
