using Cashif.Application.Lookup.Commands.Create;
using Cashif.Application.Lookup.Commands.Update;
using Cashif.Application.Lookup.Queries.GetAll;
using Cashif.Application.Lookup.Queries.GetAllPagination;
using Cashif.Application.SensorIssues.Commands.Create;
using Cashif.Application.SensorIssues.Commands.Delete;
using Cashif.Application.SensorIssues.Commands.Update;
using Cashif.Application.SensorIssues.Queries.GetAll;
using Cashif.Contracts.Common;
using Cashif.Contracts.Lookup;
using Cashif.Contracts.SensorIssues;
using Cashif.Domain.InteriorBodyIssues.Entity;
using Cashif.Domain.MechIssues.Entities;
using Cashif.Domain.Users.Permissions;
using Cashif.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Cashif.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InteriorBodyIssuesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.InteriorBodyIssue_Read)]
    public async Task<QueryResult<LookupDto>> GetAll(SearchCriteria search) => await _mediator.Send(new GetAllPaginationQuery<InteriorBodyIssue>(search));

    [HttpGet]
    public async Task<List<LookupDto>> GetAll()
          => await _mediator.Send(new GetAllLookupQuery<InteriorBodyIssue>());

    [HttpPost]
    [HasPermission(Permission.InteriorBodyIssue_Create)]
    public async Task<Guid> Create(LookupRequest req)
        =>await _mediator.Send(new CreateLookupCommand<InteriorBodyIssue>(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.InteriorBodyIssue_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
        => await _mediator.Send(new UpdateLookupCommand<InteriorBodyIssue>(id, req)) ? NoContent() : NotFound();


}
