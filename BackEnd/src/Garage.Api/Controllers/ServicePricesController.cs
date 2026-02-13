using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Services.Commands.UpsertServicePrice;
using Garage.Application.Terms.Commands.Create;
using Garage.Application.Terms.Commands.Update;
using Garage.Application.Terms.Queries.GetById;
using Garage.Contracts.Services;
using Garage.Contracts.Terms;
using Garage.Domain.Services.Entities;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServicePricesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ServiceDto>> GetById(CancellationToken ct)
    {
        var res = await _mediator.Send(new GetTermsByIdQuery(), ct);
        return Ok(res);
    }

    [HttpPut]
    [HasPermission(Permission.ServicePrice_Update)]
    public async Task<IActionResult> Upsert( Guid serviceId, [FromBody] ServicePriceRequest request,  CancellationToken ct)
    {
        await _mediator.Send(new UpsertServicePriceCommand(serviceId, request), ct);
        return NoContent();
    }
}
