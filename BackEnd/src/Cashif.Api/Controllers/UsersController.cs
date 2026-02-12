using Cashif.Application.Users.Queries.GetUsers;
using Cashif.Domain.Users.Permissions;
using Cashif.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cashif.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [HasPermission(Permission.Users_Read)]
    public async Task<IActionResult> Get() => Ok(await mediator.Send(new GetUsersQuery()));
}
