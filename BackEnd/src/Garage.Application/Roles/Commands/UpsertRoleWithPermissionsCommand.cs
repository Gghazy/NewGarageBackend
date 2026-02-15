using Garage.Contracts.Roles;
using MediatR;


namespace Garage.Application.Roles.Commands;

public record UpsertRoleWithPermissionsCommand(UpsertRoleRequest Request) :  IRequest<Guid>;
