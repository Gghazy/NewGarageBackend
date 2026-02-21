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
    private readonly IReadRepository<Employee> _employeeRepository;

    public LoginHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IReadRepository<Employee> employeeRepository)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _employeeRepository = employeeRepository;
    }

    public override async Task<Result<LoginResponse>> Handle(LoginQuery request, CancellationToken ct)
    {
        // Validate credentials
        var validationResult = await _identityService.ValidateUserAsync(request.Request, ct);
        if (!validationResult.Succeeded)
            return Result<LoginResponse>.Fail("Invalid credentials");

        // Get employee information
        var employee = await _employeeRepository.Query()
            .FirstOrDefaultAsync(e => e.UserId == validationResult.UserId, ct);

        // Create JWT token
        var (token, expiration) = _jwtTokenService.CreateToken(
            validationResult.UserId,
            employee?.NameAr ?? validationResult.Email,
            employee?.NameEn ?? validationResult.Email,
            validationResult.Email,
            validationResult.claims
        );

        return Result<LoginResponse>.Ok(new LoginResponse(token, expiration));
    }
}

