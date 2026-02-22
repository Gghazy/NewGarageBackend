using Garage.Api.Controllers.Common;
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
public class BranchesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BranchesController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all branches
    /// </summary>
    [HttpGet]
    [HasPermission(Permission.Branches_Read)]
    public async Task<IActionResult> GetAll()
    {
        var branches = await _mediator.Send(new GetAllBranchesQuery());
        return Success(branches);
    }

    /// <summary>
    /// Searches branches with pagination
    /// </summary>
    [HttpPost("pagination")]
    [HasPermission(Permission.Branches_Read)]
    public async Task<IActionResult> Search(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllBranchesBySearchQuery(search));
        return Success(result);
    }

    /// <summary>
    /// Creates a new branch
    /// </summary>
    [HttpPost]
    [HasPermission(Permission.Branches_Create)]
    public async Task<IActionResult> Create(CreateBranchRequest request)
    {
        var result = await _mediator.Send(new CreateBranchCommand(request));
        return HandleResult(result, "Branch.Created");
    }

    /// <summary>
    /// Updates an existing branch
    /// </summary>
    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Branches_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateBranchRequest request)
    {
        var result = await _mediator.Send(new UpdateBranchCommand(id, request));
        return HandleResult(result, "Branch.Updated");
    }

    /// <summary>
    /// Deletes a branch
    /// </summary>
    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Branches_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBranchCommand(id));
        return HandleResult(result, "Branch.Deleted");
    }
}

