using System;
using System.Linq;
using System.Threading.Tasks;
using Cashif.Domain.Users.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Cashif.Infrastructure.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
                return Task.CompletedTask;

            // Use the ClaimType defined in Permission to match the claims added to the user/token
            var permissions = context.User.FindAll(Permission.ClaimType).Select(c => c.Value);
            if (permissions.Contains(requirement.Permission, StringComparer.OrdinalIgnoreCase))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string Permission { get; }
        public PermissionRequirement(string permission) => Permission = permission;
    }
}
