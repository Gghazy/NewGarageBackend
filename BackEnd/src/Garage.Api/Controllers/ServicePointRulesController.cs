using Garage.Api.Controllers.Common;
using Garage.Application.ServicePointRules.Commands.Create;
using Garage.Application.ServicePointRules.Commands.Delete;
using Garage.Application.ServicePointRules.Commands.Update;
using Garage.Application.ServicePointRules.Queries.GetAllBySearch;
using Garage.Contracts.ServicePointRules;
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
public class ServicePointRulesController(IMediator mediator, IStringLocalizer localizer) : ApiControllerBase(localizer)
{
    [HttpPost("pagination")]
    [HasAnyPermission(Permission.ServicePointRule_Read)]
    public async Task<IActionResult> GetAll([FromBody] ServicePointRuleFilterDto search, CancellationToken ct)
    {
        var result = await mediator.Send(new GetAllServicePointRulesBySearchQuery(search), ct);
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.ServicePointRule_Create)]
    public async Task<IActionResult> Create([FromBody] ServicePointRuleRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new CreateServicePointRuleCommand(request), ct);
        return HandleResult(result, "ServicePointRule.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.ServicePointRule_Update)]
    public async Task<IActionResult> Update(Guid id, ServicePointRuleRequest request)
    {
        var result = await mediator.Send(new UpdateServicePointRuleCommand(id, request));
        return HandleResult(result, "ServicePointRule.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.ServicePointRule_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await mediator.Send(new DeleteServicePointRuleCommand(id));
        return HandleResult(result, "ServicePointRule.Deleted");
    }
}
