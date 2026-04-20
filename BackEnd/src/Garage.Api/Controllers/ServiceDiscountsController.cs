using Garage.Api.Controllers.Common;
using Garage.Application.ServiceDiscounts.Commands.Create;
using Garage.Application.ServiceDiscounts.Commands.Delete;
using Garage.Application.ServiceDiscounts.Commands.Update;
using Garage.Application.ServiceDiscounts.Queries.GetAllBySearch;
using Garage.Contracts.ServiceDiscounts;
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
public class ServiceDiscountsController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasAnyPermission(Permission.ServiceDiscount_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetAll([FromBody] ServiceDiscountFilterDto search, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllServiceDiscountsBySearchQuery(search), ct);
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.ServiceDiscount_Create)]
    public async Task<IActionResult> Create([FromBody] ServiceDiscountRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateServiceDiscountCommand(request), ct);
        return HandleResult(result, "ServiceDiscount.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.ServiceDiscount_Update)]
    public async Task<IActionResult> Update(Guid id, ServiceDiscountRequest request)
    {
        var result = await mediator.Send(new UpdateServiceDiscountCommand(id, request));
        return HandleResult(result, "ServiceDiscount.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.ServiceDiscount_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteServiceDiscountCommand(id));
        return HandleResult(result, "ServiceDiscount.Deleted");
    }
}
