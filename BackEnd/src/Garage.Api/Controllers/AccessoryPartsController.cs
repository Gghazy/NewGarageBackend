using Garage.Api.Controllers.Common;
using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Delete;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.AccessoryParts.Entity;
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
public class AccessoryPartsController(IMediator mediator, IStringLocalizer localizer)
    : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.AccessoryPart_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetAllPaginationQuery<AccessoryPart>(search));
        return Success(result);
    }

    [HttpGet]
    [HasPermission(Permission.AccessoryPart_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllLookupQuery<AccessoryPart>());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.AccessoryPart_Create)]
    public async Task<IActionResult> Create(LookupRequest req)
    {
        var result = await mediator.Send(new CreateLookupCommand<AccessoryPart>(req));
        return Success(result);
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.AccessoryPart_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
    {
        var updated = await mediator.Send(new UpdateLookupCommand<AccessoryPart>(id, req));
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.AccessoryPart_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLookupCommand<AccessoryPart>(id));
        return HandleResult(result, "Lookup.Deleted");
    }
}
