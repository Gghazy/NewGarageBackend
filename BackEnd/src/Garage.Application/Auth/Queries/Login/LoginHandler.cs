using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Auth;
using Garage.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Auth.Queries.Login;

public class LoginHandler : BaseQueryHandler<LoginQuery, Result<LoginResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IApplicationDbContext _db;

    public LoginHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IApplicationDbContext db)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _db = db;
    }

    public override async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken ct)
    {
        // Validate credentials
        var validationResult = await _identityService.ValidateUserAsync(request.Request, ct);
        if (!validationResult.Succeeded)
            return Result<LoginResponse>.Fail("Invalid credentials");

        // Get employee information with branches
        var employee = await _db.Employees
            .AsNoTracking()
            .Where(e => e.UserId == validationResult.UserId)
            .Select(e => new
            {
                e.NameAr,
                e.NameEn,
                BranchIds = e.Branches.Select(b => b.BranchId).ToList()
            })
            .FirstOrDefaultAsync(ct);

        // Create JWT token
        var (token, expiration) = _jwtTokenService.CreateToken(
            validationResult.UserId,
            employee?.NameAr ?? validationResult.Email,
            employee?.NameEn ?? validationResult.Email,
            validationResult.Email,
            validationResult.claims,
            employee?.BranchIds
        );

        return Result<LoginResponse>.Ok(new LoginResponse(token, expiration));
    }
}

