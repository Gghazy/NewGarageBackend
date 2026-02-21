using Garage.Api.Controllers.Common;
using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Delete;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.InsideAndDecorParts.Entity;
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
public class InsideAndDecorPartsController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.InsideAndDecorPart_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetAllPaginationQuery<InsideAndDecorPart>(search));
        return Success(result);
    }

    [HttpGet]
    [HasPermission(Permission.InsideAndDecorPart_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllLookupQuery<InsideAndDecorPart>());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.InsideAndDecorPart_Create)]
    public async Task<IActionResult> Create(LookupRequest req)
    {
        var result = await mediator.Send(new CreateLookupCommand<InsideAndDecorPart>(req));
        return Success(result);
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.InsideAndDecorPart_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
    {
        var updated = await mediator.Send(new UpdateLookupCommand<InsideAndDecorPart>(id, req));
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.InsideAndDecorPart_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLookupCommand<InsideAndDecorPart>(id));
        return HandleResult(result, "Lookup.Deleted");
    }
}
