using Garage.Api.Controllers.Common;
using Garage.Application.Profile.Commands.ChangePassword;
using Garage.Application.Profile.Commands.UpdateProfile;
using Garage.Application.Profile.Queries.GetMyProfile;
using Garage.Contracts.Profile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProfileController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyProfile()
    {
        var profile = await _mediator.Send(new GetMyProfileQuery());
        if (profile is null)
            return NotFoundMessage("Employee.NotFound");

        return Success(profile);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile(UpdateProfileRequest request)
    {
        var result = await _mediator.Send(new UpdateProfileCommand(request));
        return HandleResult(result);
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var result = await _mediator.Send(new ChangePasswordCommand(request));
        return HandleResult(result);
    }
}
