using Microsoft.AspNetCore.Authorization;

namespace Garage.Infrastructure.Authorization;

public class HasAnyPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "perm-any:";

    public HasAnyPermissionAttribute(params string[] permissions)
    {
        Policy = PolicyPrefix + string.Join("|", permissions);
    }
}
