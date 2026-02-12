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
using Garage.Domain.MechIssues.Entities;
using Garage.Domain.MechIssueTypes.Entity;
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
public class MechIssueTypesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.MechIssueType_Read)]
    public async Task<QueryResult<LookupDto>> GetAll(SearchCriteria search) => await _mediator.Send(new GetAllPaginationQuery<MechIssueType>(search));

    [HttpGet]
    public async Task<List<LookupDto>> GetAll()
          => await _mediator.Send(new GetAllLookupQuery<MechIssueType>());

    [HttpPost]
    [HasPermission(Permission.MechIssueType_Create)]
    public async Task<Guid> Create(LookupRequest req)
        =>await _mediator.Send(new CreateLookupCommand<MechIssueType>(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.MechIssueType_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
        => await _mediator.Send(new UpdateLookupCommand<MechIssueType>(id, req)) ? NoContent() : NotFound();


}

