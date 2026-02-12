using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Contracts.Auth;
using MediatR;
namespace Garage.Application.Auth.Queries.Login;
public class LoginHandler(IIdentityService identity, IJwtTokenService jwt) : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken ct)
    {
        var res = await identity.ValidateUserAsync(request.Request, ct);
        if (!res.Succeeded) return Result<LoginResponse>.Fail("Invalid credentials");
        var (token, exp) = jwt.CreateToken(res.UserId, res.Email,res.claims);
        return Result<LoginResponse>.Ok(new LoginResponse(token, exp));
    }
}

