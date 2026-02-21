using Garage.Api.Controllers.Common;
using Garage.Application.ServicePrices.Commands.Create;
using Garage.Application.ServicePrices.Commands.Delete;
using Garage.Application.ServicePrices.Commands.Update;
using Garage.Application.ServicePrices.Queries.GetAllServicePriceBySearch;
using Garage.Contracts.Common;
using Garage.Contracts.ServicePrices;
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
public class ServicePricesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasPermission(Permission.ServicePrice_Read)]
    public async Task<IActionResult> GetAll([FromBody] ServicePriceFilterDto search, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllServicePriceBySearchQuery(search), ct);
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.ServicePrice_Create)]
    public async Task<IActionResult> Create([FromBody] ServicePriceRequest request, CancellationToken ct)
    {
        await mediator.Send(new CreateServicePriceCommand(request), ct);
        return NoContent();
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.ServicePrice_Update)]
    public async Task<IActionResult> Update(Guid id, ServicePriceRequest request)
    {
        var result = await mediator.Send(new UpdateServicePriceCommand(id, request));
        return HandleResult(result, "ServicePrice.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.ServicePrice_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteServicePriceCommand(id));
        return HandleResult(result, "ServicePrice.Deleted");
    }
}
