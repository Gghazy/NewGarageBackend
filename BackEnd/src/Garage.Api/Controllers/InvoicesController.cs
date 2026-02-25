using Garage.Api.Controllers.Common;
using Garage.Application.Invoices.Commands.AddPayment;
using Garage.Application.Invoices.Commands.ChangeStatus;
using Garage.Application.Invoices.Commands.Create;
using Garage.Application.Invoices.Commands.CreateFromExamination;
using Garage.Application.Invoices.Commands.Delete;
using Garage.Application.Invoices.Commands.RefundPayment;
using Garage.Application.Invoices.Commands.Update;
using Garage.Application.Invoices.Queries.GetAll;
using Garage.Application.Invoices.Queries.GetById;
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

    [HttpPost]
    [HasPermission(Permission.Invoice_Create)]
    public async Task<IActionResult> Create(CreateInvoiceRequest request)
    {
        var result = await _mediator.Send(new CreateInvoiceCommand(request));
        return HandleResult(result, "Invoice.Created");
    }

    [HttpPost("from-examination/{examinationId:Guid}")]
    [HasPermission(Permission.Invoice_Create)]
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

    [HttpPost("{id:Guid}/issue")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> Issue(Guid id)
    {
        var result = await _mediator.Send(new IssueInvoiceCommand(id));
        return HandleResult(result, "Invoice.Issued");
    }

    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Invoice_Update)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason = null)
    {
        var result = await _mediator.Send(new CancelInvoiceCommand(id, reason));
        return HandleResult(result, "Invoice.Cancelled");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Invoice_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteInvoiceCommand(id));
        return HandleResult(result, "Invoice.Deleted");
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
}
