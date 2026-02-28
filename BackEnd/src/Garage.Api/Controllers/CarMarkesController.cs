using Garage.Api.Controllers.Common;
using Garage.Application.CarMarks.Commands.Create;
using Garage.Application.CarMarks.Commands.Update;
using Garage.Application.CarMarks.Queries.GetAll;
using Garage.Application.CarMarks.Queries.GetAllPagination;
using Garage.Application.CarMarks.Queries.GetByManufacturer;
using Garage.Application.Lookup.Commands.Delete;
using Garage.Contracts.CarMarks;
using Garage.Contracts.Common;
using Garage.Domain.CarMarkes.Entity;
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
public class CarMarkesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasAnyPermission(Permission.CarMark_Read, Permission.Examination_Read, Permission.ServicePrice_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await mediator.Send(new GetAllCarMarksPaginationQuery(search));
        return Success(result);
    }

    [HttpGet]
    [HasAnyPermission(Permission.CarMark_Read, Permission.Examination_Read, Permission.ServicePrice_Read)]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllCarMarksQuery());
        return Success(result);
    }

    [HttpGet("by-manufacturer/{manufacturerId:Guid}")]
    [HasAnyPermission(Permission.CarMark_Read, Permission.Examination_Read, Permission.ServicePrice_Read)]
    public async Task<IActionResult> GetByManufacturer(Guid manufacturerId)
    {
        var result = await mediator.Send(new GetCarMarksByManufacturerQuery(manufacturerId));
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.CarMark_Create)]
    public async Task<IActionResult> Create(CarMarkRequest req)
    {
        var result = await mediator.Send(new CreateCarMarkCommand(req));
        return HandleResult(result, "CarMark.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.CarMark_Update)]
    public async Task<IActionResult> Update(Guid id, CarMarkRequest req)
    {
        var updated = await mediator.Send(new UpdateCarMarkCommand(id, req));
        return updated ? SuccessMessage("CarMark.Updated") : NotFound();
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.CarMark_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteLookupCommand<CarMark>(id));
        return HandleResult(result, "CarMark.Deleted");
    }
}
