using Garage.Api.Controllers.Common;
using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Delete;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.RoadTestIssues.Entity;
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
public class RoadTestIssueTypesController(IMediator mediator, IStringLocalizer localizer)
    : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.RoadTestIssueType_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetAllPaginationQuery<RoadTestIssueType>(search));
        return Success(result);
    }

    [HttpGet]
    [HasPermission(Permission.RoadTestIssueType_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllLookupQuery<RoadTestIssueType>());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.RoadTestIssueType_Create)]
    public async Task<IActionResult> Create(LookupRequest req)
    {
        var result = await mediator.Send(new CreateLookupCommand<RoadTestIssueType>(req));
        return Success(result);
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.RoadTestIssueType_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
    {
        var updated = await mediator.Send(new UpdateLookupCommand<RoadTestIssueType>(id, req));
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.RoadTestIssueType_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLookupCommand<RoadTestIssueType>(id));
        return HandleResult(result, "Lookup.Deleted");
    }
}
