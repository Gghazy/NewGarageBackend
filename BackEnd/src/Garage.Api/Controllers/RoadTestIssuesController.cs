using Garage.Api.Controllers.Common;
using Garage.Application.RoadTestIssues.Commands.Create;
using Garage.Application.RoadTestIssues.Commands.Delete;
using Garage.Application.RoadTestIssues.Commands.Update;
using Garage.Application.RoadTestIssues.Queries.GetAll;
using Garage.Application.RoadTestIssues.Queries.GetAllList;
using Garage.Contracts.Common;
using Garage.Contracts.RoadTestIssues;
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
public class RoadTestIssuesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasAnyPermission(Permission.RoadTestIssue_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetRoadTestIssueQuery(search));
        return Success(result);
    }

    [HttpGet]
    [HasAnyPermission(Permission.RoadTestIssue_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllRoadTestIssuesQuery());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.RoadTestIssue_Create)]
    public async Task<IActionResult> Create(RoadTestIssueRequest req)
    {
        var result = await mediator.Send(new CreateRoadTestIssueCommand(req));
        return HandleResult(result, "RoadTestIssue.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.RoadTestIssue_Update)]
    public async Task<IActionResult> Update(Guid id, RoadTestIssueRequest req)
    {
        var result = await mediator.Send(new UpdateRoadTestIssueCommand(id, req));
        return HandleResult(result, "RoadTestIssue.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.RoadTestIssue_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteRoadTestIssueCommand(id));
        return HandleResult(result, "RoadTestIssue.Deleted");
    }
}
