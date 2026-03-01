using Garage.Application.Common;
using Garage.Contracts.Roles;
using MediatR;

namespace Garage.Application.Roles.Commands.Update;

public sealed record UpdateRoleCommand(Guid Id, UpdateRoleRequest Request) : IRequest<Result<Guid>>;
