using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Application.SensorIssues.Commands.Create;
using Garage.Application.SensorIssues.Commands.Delete;
using Garage.Application.SensorIssues.Commands.Update;
using Garage.Application.SensorIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Contracts.SensorIssues;
using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.MechIssues.Entities;
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
public class AccessoryIssuesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.AccessoryIssue_Read)]
    public async Task<QueryResult<LookupDto>> GetAll(SearchCriteria search) => await _mediator.Send(new GetAllPaginationQuery<AccessoryIssue>(search));

    [HttpGet]
    public async Task<List<LookupDto>> GetAll()
          => await _mediator.Send(new GetAllLookupQuery<AccessoryIssue>());

    [HttpPost]
    [HasPermission(Permission.AccessoryIssue_Create)]
    public async Task<Guid> Create(LookupRequest req)
        =>await _mediator.Send(new CreateLookupCommand<AccessoryIssue>(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.AccessoryIssue_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
        => await _mediator.Send(new UpdateLookupCommand<AccessoryIssue>(id, req)) ? NoContent() : NotFound();


}

