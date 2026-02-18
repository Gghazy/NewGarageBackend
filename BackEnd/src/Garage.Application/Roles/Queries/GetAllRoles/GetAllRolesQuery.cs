using Garage.Contracts.Roles;
using MediatR;


namespace Garage.Application.Roles.Queries.GetAllRoles;

public sealed record GetAllRolesQuery : IRequest<List<RoleDto>>;
