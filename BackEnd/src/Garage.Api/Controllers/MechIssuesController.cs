using Garage.Application.Common;
using Garage.Application.Lookup.Commands.Create;
using Garage.Application.Lookup.Commands.Update;
using Garage.Application.Lookup.Queries.GetAll;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Application.MechIssues.Commands.Create;
using Garage.Application.MechIssues.Commands.Update;
using Garage.Application.MechIssues.Queries.GetAll;
using Garage.Application.SensorIssues.Commands.Create;
using Garage.Application.SensorIssues.Commands.Delete;
using Garage.Application.SensorIssues.Commands.Update;
using Garage.Application.SensorIssues.Queries.GetAll;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Contracts.MechIssues;
using Garage.Contracts.SensorIssues;
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

