using Garage.Application.Services.Commands.Create;
using Garage.Application.Services.Commands.Update;
using Garage.Application.Services.Queries.GetAll;
using Garage.Application.Services.Queries.GetAllPagination;
using Garage.Application.Services.Queries.GetServiceById;
using Garage.Contracts.Common;
using Garage.Contracts.Services;
using Garage.Domain.Services.Enums;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ServicesController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    [HasPermission(Permission.Service_Read)]
    public async Task<ActionResult<ServiceDto>> GetById(Guid id, CancellationToken ct)
    {
        var res = await mediator.Send(new GetServiceByIdQuery(id), ct);
        return Ok(res);
    }

    [HttpGet]
    public async Task<ActionResult<List<ServiceDto>>> GetAll(CancellationToken ct)
    {
        var res = await mediator.Send(new GetAllServiceQuery(), ct);
        return Ok(res);
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Service_Read)]
    public async Task<ActionResult<QueryResult<ServiceDto>>> GetAll([FromBody] SearchCriteria search, CancellationToken ct)
    {
        var res = await mediator.Send(new GetAllServiceBySearchQuery(search), ct);
        return Ok(res);
    }

    [HttpPost]
    [HasPermission(Permission.Service_Read)]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateServiceRequest request, CancellationToken ct)
    {
        var id = await mediator.Send(new CreateServiceCommand(request), ct);

        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [HasPermission(Permission.Service_Read)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateServiceCommand(id, request), ct);
        return Ok();
    }

    [HttpGet("stages")]
    public IActionResult GetStages()
    {
        var stages = Stage.List
            .Select(s => new {
                Id = s.Value,
                name = s.Name,
                nameAr = s.NameAr
            });

        return Ok(stages);
    }

}
