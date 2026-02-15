using Garage.Application.ServicePrices.Commands.Create;
using Garage.Application.ServicePrices.Commands.Update;
using Garage.Application.ServicePrices.Queries.GetAllServicePriceBySearch;
using Garage.Application.Terms.Queries.GetById;
using Garage.Contracts.Common;
using Garage.Contracts.ServicePrices;
using Garage.Contracts.Services;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ServicePricesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{

    [HttpPost]
    [HasPermission(Permission.ServicePrice_Update)]
    public async Task<IActionResult> Create([FromBody] ServicePriceRequest request,  CancellationToken ct)
    {
        await _mediator.Send(new CreateServicePriceCommand(request), ct);
        return NoContent();
    }
    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.ServicePrice_Update)]
    public async Task<IActionResult> Update(Guid id, ServicePriceRequest request)
    {
        var res = await _mediator.Send(new UpdateServicePriceCommand(id, request));
        return Ok(new ApiMessage(T["SensorIssue.Updated"]!));
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.ServicePrice_Read)]
    public async Task<ActionResult<QueryResult<ServicePriceDto>>> GetAll(
     [FromBody] ServicePriceFilterDto search,
     CancellationToken ct)
    {
        var res = await _mediator.Send(new GetAllServicePriceBySearchQuery(search), ct);
        return Ok(res);
    }
}
