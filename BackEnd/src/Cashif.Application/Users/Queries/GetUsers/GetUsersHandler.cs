using Cashif.Application.Abstractions;
using Cashif.Contracts.Users;
using MediatR;
namespace Cashif.Application.Users.Queries.GetUsers;
public class GetUsersHandler(IIdentityService identity) : IRequestHandler<GetUsersQuery, IReadOnlyList<UserDto>>
{
    public async Task<IReadOnlyList<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        => await identity.ListUsersAsync(ct);
}
