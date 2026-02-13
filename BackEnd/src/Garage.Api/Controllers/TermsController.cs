using Garage.Application.Common;
using Garage.Application.MechIssues.Commands.Create;
using Garage.Application.MechIssues.Commands.Update;
using Garage.Application.Services.Queries.GetServiceById;
using Garage.Application.Terms.Commands.Create;
using Garage.Application.Terms.Commands.Update;
using Garage.Application.Terms.Queries.GetById;
using Garage.Contracts.MechIssues;
using Garage.Contracts.Services;
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
public class TermsController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ServiceDto>> GetById(CancellationToken ct)
    {
        var res = await _mediator.Send(new GetTermsByIdQuery(), ct);
        return Ok(res);
    }

    [HttpPost]
    [HasPermission(Permission.Term_Create)]
    public async Task<Result<Guid>> Create(CreateTermsRequest req)
        =>await _mediator.Send(new CreateTermCommand(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Term_Update)]
    public async Task<Result<Guid>> Update(Guid id, CreateTermsRequest req)
        => await _mediator.Send(new UpdateTermCommand(id, req));


}

