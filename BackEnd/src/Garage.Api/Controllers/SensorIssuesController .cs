using Garage.Api.Controllers.Common;
using Garage.Application.SensorIssues.Commands.Create;
using Garage.Application.SensorIssues.Commands.Delete;
using Garage.Application.SensorIssues.Commands.Update;
using Garage.Application.SensorIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Contracts.SensorIssues;
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
public class SensorIssuesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.SensorIssue_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetSensorIssueQuery(search));
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.SensorIssue_Create)]
    public async Task<IActionResult> Create(CreateSensorIssueRequest request)
    {
        var result = await mediator.Send(new CreateSensorIssueCommand(request));
        return HandleResult(result, "SensorIssue.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.SensorIssue_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateSensorIssueRequest request)
    {
        var result = await mediator.Send(new UpdateSensorIssueCommand(id, request));
        return HandleResult(result, "SensorIssue.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.SensorIssue_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteSensorIssueCommand(id));
        return HandleResult(result, "SensorIssue.Deleted");
    }
}
