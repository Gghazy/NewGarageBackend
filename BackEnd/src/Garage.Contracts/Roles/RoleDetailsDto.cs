
namespace Garage.Contracts.Roles;

public sealed record RoleDetailsDto( Guid Id, string Name, List<string> Permissions);
