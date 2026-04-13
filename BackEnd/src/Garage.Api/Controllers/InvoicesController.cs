using Garage.Api.Controllers.Common;
using Garage.Application.Invoices.Commands.AddPayment;
using Garage.Application.Invoices.Commands.ChangeStatus;
using Garage.Application.Invoices.Commands.Create;
using Garage.Application.Invoices.Commands.CreateFromExamination;
using Garage.Application.Invoices.Commands.RefundPayment;
using Garage.Application.Invoices.Commands.SetDiscount;
using Garage.Application.Invoices.Commands.Update;
using Garage.Application.Invoices.Queries.GetAll;
using Garage.Application.Invoices.Queries.GetByExamination;
using Garage.Application.Invoices.Queries.GetById;
using Garage.Application.Invoices.Queries.GetConsolidated;
using Garage.Application.Invoices.Queries.GetHistory;
using Garage.Application.Invoices.Queries.GetRevenue;
using Garage.Application.Invoices.Queries.GetRevenueComparison;
using Garage.Application.Abstractions;
using Garage.Contracts.Common;
using Garage.Contracts.Invoices;
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
public class InvoicesController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public InvoicesController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Invoice_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllInvoicesQuery(search));
        return Success(result);
    }

    [HttpGet("{id:Guid}")]
    [HasPermission(Permission.Invoice_Read)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpGet("by-examination/{examinationId:Guid}")]
    [HasAnyPermission(Permission.Invoice_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetByExamination(Guid examinationId)
    {
        var result = await _mediator.Send(new GetInvoicesByExaminationQuery(examinationId));
        return Success(result);
    }

    [HttpGet("consolidated/{examinationId:Guid}")]
    [HasAnyPermission(Permission.Invoice_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetConsolidated(Guid examinationId)
    {
        var result = await _mediator.Send(new GetConsolidatedByExaminationQuery(examinationId));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.Invoice_Create)]
    public async Task<IActionResult> Create(CreateInvoiceRequest request)
    {
        var result = await _mediator.Send(new CreateInvoiceCommand(request));
        return HandleResult(result, "Invoice.Created");
    }

    [HttpPost("from-examination/{examinationId:Guid}")]
    [HasAnyPermission(Permission.Invoice_Create, Permission.Examination_Create)]
    public async Task<IActionResult> CreateFromExamination(Guid examinationId)
    {
        var result = await _mediator.Send(new CreateInvoiceFromExaminationCommand(examinationId));
        return HandleResult(result, "Invoice.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateInvoiceRequest request)
    {
        var result = await _mediator.Send(new UpdateInvoiceCommand(id, request));
        return HandleResult(result, "Invoice.Updated");
    }

    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelInvoiceCommand(id, reason));
        return HandleResult(result, "Invoice.Cancelled");
    }

    [HttpPut("{id:Guid}/discount")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> SetDiscount(Guid id, [FromBody] decimal amount)
    {
        var result = await _mediator.Send(new SetInvoiceDiscountCommand(id, amount));
        return HandleResult(result, "Invoice.Updated");
    }

    [HttpPost("{id:Guid}/payments")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> AddPayment(Guid id, AddInvoicePaymentRequest request)
    {
        var result = await _mediator.Send(new AddInvoicePaymentCommand(id, request));
        return HandleResult(result, "Invoice.PaymentAdded");
    }

    [HttpPost("{id:Guid}/refunds")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> AddRefund(Guid id, AddInvoicePaymentRequest request)
    {
        var result = await _mediator.Send(new RefundInvoicePaymentCommand(id, request));
        return HandleResult(result, "Invoice.RefundAdded");
    }

    [HttpGet("revenue")]
    [HasPermission(Permission.Dashboard_Revenue)]
    public async Task<IActionResult> GetRevenue([FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetRevenueQuery(from, to, branchId));
        return Success(result);
    }

    [HttpGet("revenue/comparison")]
    [HasPermission(Permission.Dashboard_Revenue)]
    public async Task<IActionResult> GetRevenueComparison(
        [FromQuery] DateTime from1,
        [FromQuery] DateTime to1,
        [FromQuery] DateTime from2,
        [FromQuery] DateTime to2,
        [FromQuery] Guid? branchId)
    {
        var result = await _mediator.Send(new GetRevenueComparisonQuery(from1, to1, from2, to2, branchId));
        return Success(result);
    }

    [HttpGet("{id:Guid}/history")]
    [HasPermission(Permission.Invoice_Read)]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var result = await _mediator.Send(new GetInvoiceHistoryQuery(id));
        return Success(result);
    }

    [HttpGet("{id:Guid}/view")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicView(Guid id, [FromQuery] string lang, [FromServices] IInvoiceHtmlRenderer renderer)
    {
        var inv = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (inv is null) return NotFound();
        return Content(renderer.RenderPublicView(inv, lang ?? "ar"), "text/html");
    }
}
