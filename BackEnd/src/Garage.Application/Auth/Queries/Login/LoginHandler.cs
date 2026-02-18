using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Contracts.Auth;
using Garage.Domain.Employees.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Garage.Application.Auth.Queries.Login;
public class LoginHandler(IIdentityService identity, IJwtTokenService jwt,IReadRepository<Employee> _repo) : IRequestHandler<LoginQuery, Result<LoginResponse>>
{
    public async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken ct)
    {
        var res = await identity.ValidateUserAsync(request.Request, ct);
        if (!res.Succeeded) return Result<LoginResponse>.Fail("Invalid credentials");

        var employee = await _repo.Query()
            .FirstOrDefaultAsync(e => e.UserId == res.UserId, ct);


        var (token, exp) = jwt.CreateToken(res.UserId, employee?.NameAr?? res.Email, employee?.NameEn ?? res.Email, res.Email, res.claims);
        return Result<LoginResponse>.Ok(new LoginResponse(token, exp));
    }
}

