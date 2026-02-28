using Microsoft.AspNetCore.Authorization;

namespace Garage.Infrastructure.Authorization;

public class AnyPermissionRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }

    public AnyPermissionRequirement(params string[] permissions)
        => Permissions = permissions;
}
