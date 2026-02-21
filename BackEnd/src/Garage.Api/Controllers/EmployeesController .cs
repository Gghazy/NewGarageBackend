using Garage.Api.Controllers.Common;
using Garage.Application.Employees.Commands.Create;
using Garage.Application.Employees.Commands.Delete;
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
public class EmployeesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public EmployeesController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [HasPermission(Permission.Employees_Create)]
    public async Task<IActionResult> Create([FromBody] EmployeeRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateEmployeeCommand(request), ct);
        return HandleResult(result, "Employee.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Employees_Update)]
    public async Task<IActionResult> Update(Guid id, EmployeeRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateEmployeeCommand(id, request), ct);
        return HandleResult(result, "Employee.Updated");
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Employees_Read)]
    public async Task<IActionResult> GetAllPaginated([FromBody] SearchCriteria search, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllEmployeesBySearchQuery(search), ct);
        return Success(result);
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Employees_Delete)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteEmployeeCommand(id), ct);
        return HandleResult(result, "Employee.Deleted");
    }
}
