using Garage.Api.Controllers.Common;
using Garage.Application.Examinations.Commands.AddPayment;
using Garage.Application.Examinations.Commands.ChangeStatus;
using Garage.Application.Examinations.Commands.Create;
using Garage.Application.Examinations.Commands.Update;
using Garage.Application.Examinations.Queries.GetAll;
using Garage.Application.Examinations.Queries.GetById;
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

    /// <summary>Gets a paginated list of examinations with optional text search.</summary>
    [HttpPost("pagination")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllExaminationsQuery(search));
        return Success(result);
    }

    /// <summary>Gets a single examination by ID.</summary>
    [HttpGet("{id:Guid}")]
    [HasPermission(Permission.Examination_Read)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetExaminationByIdQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    /// <summary>
    /// Creates a new examination (Draft status).
    /// If ClientId is null a new client is created; otherwise the existing client is updated.
    /// Items are optional at creation — required before calling Start.
    /// </summary>
    [HttpPost]
    [HasPermission(Permission.Examination_Create)]
    public async Task<IActionResult> Create(CreateExaminationRequest request)
    {
        var result = await _mediator.Send(new CreateExaminationCommand(request));
        return HandleResult(result, "Examination.Created");
    }

    /// <summary>Updates an examination's metadata and optionally replaces its items (Draft only).</summary>
    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateExaminationRequest request)
    {
        var result = await _mediator.Send(new UpdateExaminationCommand(id, request));
        return HandleResult(result, "Examination.Updated");
    }

    // ── Status transitions ────────────────────────────────────────────────────

    /// <summary>Starts an examination (Draft → InProgress). Requires at least one item.</summary>
    [HttpPost("{id:Guid}/start")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Start(Guid id)
    {
        var result = await _mediator.Send(new StartExaminationCommand(id));
        return HandleResult(result, "Examination.Started");
    }

    /// <summary>Marks an examination as completed (InProgress → Completed).</summary>
    [HttpPost("{id:Guid}/complete")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Complete(Guid id)
    {
        var result = await _mediator.Send(new CompleteExaminationCommand(id));
        return HandleResult(result, "Examination.Completed");
    }

    /// <summary>Marks an examination as delivered (Completed → Delivered).</summary>
    [HttpPost("{id:Guid}/deliver")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Deliver(Guid id)
    {
        var result = await _mediator.Send(new DeliverExaminationCommand(id));
        return HandleResult(result, "Examination.Delivered");
    }

    /// <summary>Cancels an examination. Optional cancellation reason.</summary>
    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelExaminationCommand(id, reason));
        return HandleResult(result, "Examination.Cancelled");
    }

    // ── Payments ─────────────────────────────────────────────────────────────

    /// <summary>Adds a payment to an examination.</summary>
    [HttpPost("{id:Guid}/payments")]
    [HasPermission(Permission.Examination_Update)]
    public async Task<IActionResult> AddPayment(Guid id, AddPaymentRequest request)
    {
        var result = await _mediator.Send(new AddPaymentCommand(id, request));
        return HandleResult(result, "Examination.PaymentAdded");
    }
}
