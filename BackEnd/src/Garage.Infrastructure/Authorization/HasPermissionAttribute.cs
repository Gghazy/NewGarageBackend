using Microsoft.AspNetCore.Authorization;
namespace Garage.Infrastructure.Authorization;
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = PermissionPolicies.PolicyName(permission);
    }
}

