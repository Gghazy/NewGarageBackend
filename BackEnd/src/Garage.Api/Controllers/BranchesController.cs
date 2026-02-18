using Garage.Application.Branches.Commands.Create;
using Garage.Application.Branches.Commands.Delete;
using Garage.Application.Branches.Commands.Update;
using Garage.Application.Branches.Queries.GetAll;
using Garage.Application.Branches.Queries.GetAllBranchesBySearch;
using Garage.Contracts.Branches;
using Garage.Contracts.Common;
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
public class BranchesController(IMediator mediator, IStringLocalizer T) : ControllerBase
{

    [HttpGet]
    [HasPermission(Permission.Branches_Read)]
    public async Task<IActionResult> GetAll() => Ok(await mediator.Send(new GetAllBranchesQuery()));


    [HttpPost("pagination")]
    [HasPermission(Permission.Branches_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search) => Ok(await mediator.Send(new GetAllBranchesBySearchQuery(search)));

    [HttpPost]
    [HasPermission(Permission.Branches_Create)]
    public async Task<IActionResult> Create(CreateBranchRequest request)
    {
        var res = await mediator.Send(new CreateBranchCommand(request));
        if (!res.Succeeded) return BadRequest(new ApiMessage(T["Branch.Exists"]!));

        var x = T["Branch.Created"];

        return Ok(new ApiMessage(T["Branch.Created"]!));
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Branches_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateBranchRequest request)
    {
        var res = await mediator.Send(new UpdateBranchCommand(id, request));
        if (!res.Succeeded) return NotFound(new ApiMessage(T["NotFound"]!));
        return Ok(new ApiMessage(T["Branch.Updated"]!));
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Branches_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var res = await mediator.Send(new DeleteBranchCommand(id));
        if (!res.Succeeded) return NotFound(new ApiMessage(T["NotFound"]!));
        return Ok(new ApiMessage(T["Branch.Deleted"]!));
    }
}

