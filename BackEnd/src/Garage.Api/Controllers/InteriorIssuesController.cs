using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.InteriorIssues.Entity;
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
public class InteriorIssuesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.InteriorIssue_Read)]
    public async Task<QueryResult<LookupDto>> GetAll(SearchCriteria search) => await _mediator.Send(new GetAllPaginationQuery<InteriorIssue>(search));

    [HttpGet]
    public async Task<List<LookupDto>> GetAll()
          => await _mediator.Send(new GetAllLookupQuery<InteriorIssue>());

    [HttpPost]
    [HasPermission(Permission.InteriorIssue_Create)]
    public async Task<Guid> Create(LookupRequest req)
        =>await _mediator.Send(new CreateLookupCommand<InteriorIssue>(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.InteriorIssue_Update)]
    public async Task<IActionResult> Update(Guid id, LookupRequest req)
        => await _mediator.Send(new UpdateLookupCommand<InteriorIssue>(id, req)) ? NoContent() : NotFound();


}

