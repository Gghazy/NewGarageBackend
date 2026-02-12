using Cashif.Application.Auth.Commands.RegisterUser;
using Cashif.Application.Auth.Queries.Login;
using Cashif.Contracts.Auth;
using Cashif.Contracts.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cashif.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var res = await mediator.Send(new RegisterUserCommand(request));
        if (!res.Succeeded) return BadRequest(new ApiMessage(res.Error!));
        return Ok(new ApiResponse<Guid?>(res.Value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var res = await mediator.Send(new LoginQuery(request));
        if (!res.Succeeded) return Unauthorized(new ApiMessage(T["Auth.InvalidCredentials"]!));
        return Ok(res.Value);
    }
}
