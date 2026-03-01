namespace Garage.Contracts.Roles;

public sealed record UpdateRoleRequest(string RoleName, List<string> Permissions);
