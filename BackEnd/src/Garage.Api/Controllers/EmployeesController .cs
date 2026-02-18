using Garage.Application.Employees.Commands.Create;
using Garage.Application.Employees.Commands.Update;
using Garage.Application.Employees.Queries.GetAllEmployeesBySearch;
using Garage.Contracts.Common;
using Garage.Contracts.Employees;
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
public class EmployeesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost]
    [HasPermission(Permission.Employees_Create)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request, CancellationToken ct)
    {
        await _mediator.Send(new CreateEmployeeCommand(request), ct);
        return NoContent();
    }
    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Employees_Update)]
    public async Task<IActionResult> Update(Guid id, EmployeeRequest request)
    {
        var res = await _mediator.Send(new UpdateEmployeeCommand(id, request));
        return Ok(new ApiMessage(T["SensorIssue.Updated"]!));
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Employees_Read)]
    public async Task<ActionResult<QueryResult<EmployeeDto>>> GetAll(
     [FromBody] SearchCriteria search,
     CancellationToken ct)
    {
        var res = await _mediator.Send(new GetAllEmployeesBySearchQuery(search), ct);
        return Ok(res);
    }


}

