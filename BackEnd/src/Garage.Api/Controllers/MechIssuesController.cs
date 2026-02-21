using Garage.Api.Controllers.Common;
using Garage.Application.MechIssues.Commands.Create;
using Garage.Application.MechIssues.Commands.Delete;
using Garage.Application.MechIssues.Commands.Update;
using Garage.Application.MechIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Contracts.MechIssues;
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
public class MechIssuesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.MechIssue_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetMechIssueQuery(search));
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.MechIssue_Create)]
    public async Task<IActionResult> Create(MechIssueRequest req)
    {
        var result = await mediator.Send(new CreateMechIssueCommand(req));
        return HandleResult(result, "MechIssue.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.MechIssue_Update)]
    public async Task<IActionResult> Update(Guid id, MechIssueRequest req)
    {
        var result = await mediator.Send(new UpdateMechIssueCommand(id, req));
        return HandleResult(result, "MechIssue.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.MechIssue_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteMechIssueCommand(id));
        return HandleResult(result, "MechIssue.Deleted");
    }
}
