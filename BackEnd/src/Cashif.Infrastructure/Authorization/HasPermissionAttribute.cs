using Microsoft.AspNetCore.Authorization;
namespace Cashif.Infrastructure.Authorization;
public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
    {
        Policy = PermissionPolicies.PolicyName(permission);
    }
}
