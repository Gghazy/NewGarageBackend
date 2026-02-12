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
using Garage.Domain.RoadTestIssues.Entity;
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
public class RoadTestIssuesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.RoadTestIssue_Read)]
    public async Task<QueryResult<LookupDto>> GetAll(SearchCriteria search) => await _mediator.Send(new GetAllPaginationQuery<RoadTestIssue>(search));

    [HttpGet]
    public async Task<List<LookupDto>> GetAll()
          => await _mediator.Send(new GetAllLookupQuery<RoadTestIssue>());

    [HttpPost]
    [HasPermission(Permission.RoadTestIssue_Create)]
    public async Task<Guid> Create(LookupRequest req)
        =>await _mediator.Send(new CreateLookupCommand<RoadTestIssue>(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.RoadTestIssue_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
        => await _mediator.Send(new UpdateLookupCommand<RoadTestIssue>(id, req)) ? NoContent() : NotFound();


}

