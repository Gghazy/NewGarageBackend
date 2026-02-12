using Garage.Application.SensorIssues.Commands.Create;
using Garage.Application.SensorIssues.Commands.Update;
using Garage.Application.SensorIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Garage.Contracts.SensorIssues;
using Garage.Application.SensorIssues.Commands.Delete;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SensorIssuesController(IMediator mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.SensorIssue_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search) => Ok(await mediator.Send(new GetSensorIssueQuery(search)));

    [HttpPost]
    [HasPermission(Permission.SensorIssue_Create)]
    public async Task<IActionResult> Create(CreateSensorIssueRequest request)
    {
        var res = await mediator.Send(new CreateSensorIssueCommand(request));
        if (!res.Succeeded) return BadRequest(new ApiMessage(T["SensorIssue.Exists"]!));

        var x = T["SensorIssue.Created"];

        return Ok(new ApiMessage(T["SensorIssue.Created"]!));
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.SensorIssue_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateSensorIssueRequest request)
    {
        var res = await mediator.Send(new UpdateSensorIssueCommand(id, request));
        if (!res.Succeeded) return NotFound(new ApiMessage(T["NotFound"]!));
        return Ok(new ApiMessage(T["SensorIssue.Updated"]!));
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.SensorIssue_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var res = await mediator.Send(new DeleteSensorIssueCommand(id));
        if (!res.Succeeded) return NotFound(new ApiMessage(T["NotFound"]!));
        return Ok(new ApiMessage(T["SensorIssue.Deleted"]!));
    }
}

