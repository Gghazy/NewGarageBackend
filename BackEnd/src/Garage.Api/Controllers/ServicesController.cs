using Garage.Api.Controllers.Common;
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
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public sealed class ServicesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ServicesController(IMediator mediator, IStringLocalizer localizer) 
        : base(localizer)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets a service by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [HasPermission(Permission.Service_Read)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var service = await _mediator.Send(new GetServiceByIdQuery(id), ct);
        if (service is null)
            return NotFoundMessage("Service.NotFound");

        return Success(service);
    }

    /// <summary>
    /// Gets all services
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var services = await _mediator.Send(new GetAllServiceQuery(), ct);
        return Success(services);
    }

    /// <summary>
    /// Gets all services with pagination
    /// </summary>
    [HttpPost("pagination")]
    [HasPermission(Permission.Service_Read)]
    public async Task<IActionResult> GetAllPaginated([FromBody] SearchCriteria search, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllServiceBySearchQuery(search), ct);
        return Success(result);
    }

    /// <summary>
    /// Creates a new service
    /// </summary>
    [HttpPost]
    [HasPermission(Permission.Service_Create)]
    public async Task<IActionResult> Create([FromBody] CreateServiceRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateServiceCommand(request), ct);
        return HandleResult(result, "Service.Created");
    }

    /// <summary>
    /// Updates a service
    /// </summary>
    [HttpPut("{id:guid}")]
    [HasPermission(Permission.Service_Update)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateServiceRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateServiceCommand(id, request), ct);
        return HandleResult(result, "Service.Updated");
    }

    /// <summary>
    /// Gets all available service stages
    /// </summary>
    [HttpGet("stages")]
    public IActionResult GetStages()
    {
        var stages = Stage.List
            .Select(s => new {
                Id = s.Value,
                name = s.Name,
                nameAr = s.NameAr
            });

        return Success(stages);
    }
}
