using Garage.Application.Common;
using Garage.Application.Abstractions;
using MediatR;
namespace Garage.Application.Auth.Commands.RegisterUser;
public class RegisterUserHandler(IIdentityService identity) : IRequestHandler<RegisterUserCommand, Result<Guid?>>
{
    public async Task<Result<Guid?>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var (ok, err, userId) = await identity.CreateUserAsync(request.Request, ct);
        return ok ? Result<Guid?>.Ok(userId) : Result<Guid?>.Fail(err!);
    }
}

