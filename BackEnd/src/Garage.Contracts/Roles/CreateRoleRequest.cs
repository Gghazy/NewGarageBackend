namespace Garage.Contracts.Roles;

public sealed record CreateRoleRequest(string RoleName, List<string> Permissions);
