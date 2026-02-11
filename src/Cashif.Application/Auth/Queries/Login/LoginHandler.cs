using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Contracts.Auth;
using MediatR;
namespace Cashif.Application.Auth.Queries.Login;
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
