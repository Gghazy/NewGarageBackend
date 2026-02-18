using Garage.Contracts.Roles;
using MediatR;


namespace Garage.Application.Roles.Queries.GetRoleById;

public sealed record GetRoleByIdQuery(Guid Id) : IRequest<RoleDetailsDto>;
