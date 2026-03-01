using Garage.Application.Common;
using Garage.Contracts.Roles;
using MediatR;

namespace Garage.Application.Roles.Commands.Create;

public sealed record CreateRoleCommand(CreateRoleRequest Request) : IRequest<Result<Guid>>;
