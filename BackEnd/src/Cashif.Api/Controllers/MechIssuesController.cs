using Cashif.Application.Common;
using Cashif.Application.Lookup.Commands.Create;
using Cashif.Application.Lookup.Commands.Update;
using Cashif.Application.Lookup.Queries.GetAll;
using Cashif.Application.Lookup.Queries.GetAllPagination;
using Cashif.Application.MechIssues.Commands.Create;
using Cashif.Application.MechIssues.Commands.Update;
using Cashif.Application.MechIssues.Queries.GetAll;
using Cashif.Application.SensorIssues.Commands.Create;
using Cashif.Application.SensorIssues.Commands.Delete;
using Cashif.Application.SensorIssues.Commands.Update;
using Cashif.Application.SensorIssues.Queries.GetAll;
using Cashif.Contracts.Common;
using Cashif.Contracts.Lookup;
using Cashif.Contracts.MechIssues;
using Cashif.Contracts.SensorIssues;
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
public class MechIssuesController(IMediator _mediator, IStringLocalizer T) : ControllerBase
{
    [HttpPost("pagination")]
    [HasPermission(Permission.MechIssue_Read)]
    public async Task<QueryResult<MechIssueResponse>> GetAll(SearchCriteria search) => await _mediator.Send(new GetMechIssueQuery(search));

    [HttpPost]
    [HasPermission(Permission.MechIssue_Create)]
    public async Task<Result<Guid>> Create(MechIssueRequest req)
        =>await _mediator.Send(new CreateMechIssueCommand(req));

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.MechIssue_Update)]
    public async Task<Result<bool>> Update(Guid id, MechIssueRequest req)
        => await _mediator.Send(new UpdateMechIssueCommand(id, req));


}
