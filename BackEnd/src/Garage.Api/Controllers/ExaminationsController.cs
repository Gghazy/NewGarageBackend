using Garage.Api.Controllers.Common;
using Garage.Application.Examinations.Commands.ChangeStatus;
using Garage.Application.Examinations.Commands.Create;
using Garage.Application.Examinations.Commands.Delete;
using Garage.Application.Examinations.Commands.Update;
using Garage.Application.Examinations.Queries.GetAll;
using Garage.Application.Examinations.Queries.GetById;
using Garage.Application.Examinations.Queries.GetCount;
using Garage.Application.Examinations.Queries.GetServiceUsage;
using Garage.Application.Examinations.Queries.GetWorkflow;
using Garage.Contracts.Common;
using Garage.Contracts.Examinations;
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
public class ExaminationsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ExaminationsController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllExaminationsQuery(search));
        return Success(result);
    }

    [HttpGet("{id:Guid}")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationByIdQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }
    [HttpPost]
    [HasPermission(Permission.Examination_Create)]
    public async Task<IActionResult> Create(CreateExaminationRequest request)
    {
        var result = await _mediator.Send(new CreateExaminationCommand(request));
        return HandleResult(result, "Examination.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateExaminationRequest request)
    {
        var result = await _mediator.Send(new UpdateExaminationCommand(id, request));
        return HandleResult(result, "Examination.Updated");
    }


    [HttpPost("{id:Guid}/start")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Start(Guid id)
    {
        var result = await _mediator.Send(new StartExaminationCommand(id));
        return HandleResult(result, "Examination.Started");
    }

    [HttpPost("{id:Guid}/complete")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _mediator.Send(new CompleteExaminationCommand(id));
        return HandleResult(result, "Examination.Completed");
    }

    [HttpPost("{id:Guid}/deliver")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Deliver(Guid id)
    {
        var result = await _mediator.Send(new DeliverExaminationCommand(id));
        return HandleResult(result, "Examination.Delivered");
    }

    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelExaminationCommand(id, reason));
        return HandleResult(result, "Examination.Cancelled");
    }


    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Examination_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteExaminationCommand(id));
        return HandleResult(result, "Examination.Deleted");
    }

    [HttpGet("{id:Guid}/workflow")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetWorkflow(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationWorkflowQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpGet("count")]
    [HasPermission(Permission.Dashboard_Examinations)]
    public async Task<IActionResult> GetCount([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetExaminationsCountQuery(from, to, branchId));
        return Success(result);
    }

    [HttpGet("service-usage")]
    [HasPermission(Permission.Dashboard_Examinations)]
    public async Task<IActionResult> GetServiceUsage([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetServiceUsageQuery(from, to, branchId));
        return Success(result);
    }
}
