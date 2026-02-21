using Garage.Api.Controllers.Common;
using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Delete;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.ClientResources.Entities;
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
public class ClientResourcesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.ClientResource_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetAllPaginationQuery<ClientResource>(search));
        return Success(result);
    }

    [HttpGet]
    [HasPermission(Permission.ClientResource_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllLookupQuery<ClientResource>());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.ClientResource_Create)]
    public async Task<IActionResult> Create(LookupRequest req)
    {
        var result = await mediator.Send(new CreateLookupCommand<ClientResource>(req));
        return Success(result);
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.ClientResource_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
    {
        var updated = await mediator.Send(new UpdateLookupCommand<ClientResource>(id, req));
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.ClientResource_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLookupCommand<ClientResource>(id));
        return HandleResult(result, "Lookup.Deleted");
    }
}
