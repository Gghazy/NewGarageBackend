using Cashif.Contracts.Users;
using MediatR;
namespace Cashif.Application.Users.Queries.GetUsers;
public record GetUsersQuery : IRequest<IReadOnlyList<UserDto>>;
