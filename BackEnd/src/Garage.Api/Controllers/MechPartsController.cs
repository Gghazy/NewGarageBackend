using Garage.Api.Controllers.Common;
using Garage.Application.MechParts.Commands.Create;
using Garage.Application.MechParts.Commands.Delete;
using Garage.Application.MechParts.Commands.Update;
using Garage.Application.MechParts.Queries.GetAll;
using Garage.Application.MechParts.Queries.GetAllList;
using Garage.Contracts.Common;
using Garage.Contracts.MechParts;
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
public class MechPartsController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasAnyPermission(Permission.MechPart_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetMechPartQuery(search));
        return Success(result);
    }

    [HttpGet]
    [HasAnyPermission(Permission.MechPart_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllMechPartsQuery());
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.MechPart_Create)]
    public async Task<IActionResult> Create(MechPartRequest req)
    {
        var result = await mediator.Send(new CreateMechPartCommand(req));
        return HandleResult(result, "MechPart.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.MechPart_Update)]
    public async Task<IActionResult> Update(Guid id, MechPartRequest req)
    {
        var result = await mediator.Send(new UpdateMechPartCommand(id, req));
        return HandleResult(result, "MechPart.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.MechPart_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteMechPartCommand(id));
        return HandleResult(result, "MechPart.Deleted");
    }
}
