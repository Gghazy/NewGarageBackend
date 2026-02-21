using Garage.Api.Controllers.Common;
using Garage.Application.Auth.Commands.RegisterUser;
using Garage.Application.Auth.Queries.Login;
using Garage.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var result = await _mediator.Send(new RegisterUserCommand(request));
        return HandleResult(result, "User.Created");
    }

    /// <summary>
    /// Authenticates user and returns JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _mediator.Send(new LoginQuery(request));
        if (!result.Succeeded)
            return UnauthorizedMessage("Auth.InvalidCredentials");

        return Success(result.Value);
    }
}

