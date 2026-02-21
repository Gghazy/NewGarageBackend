using Garage.Api.Controllers.Common;
using Garage.Application.Users.Queries.GetUsers;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all users
    /// </summary>
    [HttpGet]
    [HasPermission(Permission.Users_Read)]
    public async Task<IActionResult> Get()
    {
        var users = await _mediator.Send(new GetUsersQuery());
        return Success(users);
    }
}

