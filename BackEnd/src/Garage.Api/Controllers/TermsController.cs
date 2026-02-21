using Garage.Api.Controllers.Common;
using Garage.Application.Terms.Commands.Create;
using Garage.Application.Terms.Commands.Delete;
using Garage.Application.Terms.Commands.Update;
using Garage.Application.Terms.Queries.GetById;
using Garage.Contracts.Terms;
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
public class TermsController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpGet]
    public async Task<IActionResult> GetById(CancellationToken ct)
    {
        var result = await mediator.Send(new GetTermsByIdQuery(), ct);
        if (result is null)
            return NotFoundMessage("Terms.NotFound");
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.Term_Create)]
    public async Task<IActionResult> Create(CreateTermsRequest req)
    {
        var result = await mediator.Send(new CreateTermCommand(req));
        return HandleResult(result, "Terms.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Term_Update)]
    public async Task<IActionResult> Update(Guid id, CreateTermsRequest req)
    {
        var result = await mediator.Send(new UpdateTermCommand(id, req));
        return HandleResult(result, "Terms.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Term_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteTermCommand(id));
        return HandleResult(result, "Terms.Deleted");
    }
}
