using Garage.Contracts.Users;
using MediatR;
namespace Garage.Application.Users.Queries.GetUsers;
public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;

