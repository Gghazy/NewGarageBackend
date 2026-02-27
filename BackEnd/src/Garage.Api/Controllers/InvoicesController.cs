using Garage.Api.Controllers.Common;
using Garage.Application.Invoices.Commands.AddPayment;
using Garage.Application.Invoices.Commands.ChangeStatus;
using Garage.Application.Invoices.Commands.Create;
using Garage.Application.Invoices.Commands.CreateFromExamination;
using Garage.Application.Invoices.Commands.Delete;
using Garage.Application.Invoices.Commands.RefundPayment;
using Garage.Application.Invoices.Commands.SetDiscount;
using Garage.Application.Invoices.Commands.Update;
using Garage.Application.Invoices.Queries.GetAll;
using Garage.Application.Invoices.Queries.GetByExamination;
using Garage.Application.Invoices.Queries.GetById;
using Garage.Application.Invoices.Queries.GetRevenue;
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
    [HasPermission(Permission.Invoice_Read)]
    public async Task<IActionResult> GetByExamination(Guid examinationId)
    {
        var result = await _mediator.Send(new GetInvoicesByExaminationQuery(examinationId));
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

    [HttpGet("{id:Guid}/view")]
    [AllowAnonymous]
    public async Task<IActionResult> PublicView(Guid id)
    {
        var inv = await _mediator.Send(new GetInvoiceByIdQuery(id));
        if (inv is null) return NotFound();

        var itemsRows = string.Join("", inv.Items.Select((item, i) => $@"
            <tr>
                <td style='text-align:center'>{i + 1}</td>
                <td>{item.ServiceNameAr ?? item.Description}</td>
                <td style='text-align:center'>{item.Quantity}</td>
                <td style='text-align:center'>{item.UnitPrice:F2}</td>
                <td style='text-align:center'>{item.TotalPrice:F2}</td>
            </tr>"));

        var paymentsRows = string.Join("", inv.Payments.Select((p, i) => $@"
            <tr>
                <td style='text-align:center'>{i + 1}</td>
                <td>{p.CreatedAtUtc:yyyy-MM-dd}</td>
                <td style='text-align:center'>{p.Type}</td>
                <td style='text-align:center'>{p.Amount:F2} {p.Currency}</td>
                <td>{p.MethodNameAr}</td>
            </tr>"));

        var html = $@"<!DOCTYPE html>
<html dir='rtl' lang='ar'>
<head>
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>
    <title>فاتورة - {inv.InvoiceNumber ?? inv.Id.ToString()}</title>
    <style>
        * {{ margin:0; padding:0; box-sizing:border-box; }}
        body {{ font-family:'Segoe UI',Tahoma,sans-serif; font-size:14px; color:#333; padding:20px; direction:rtl; max-width:800px; margin:0 auto; background:#fff; }}
        .header {{ text-align:center; margin-bottom:24px; padding-bottom:16px; border-bottom:2px solid #ddd; }}
        .header h1 {{ font-size:24px; margin-bottom:4px; color:#333; }}
        .header .meta {{ font-size:13px; color:#888; }}
        .info-row {{ display:flex; justify-content:space-between; padding:10px 16px; border-bottom:1px solid #eee; }}
        .info-row .label {{ color:#888; font-size:13px; }}
        .info-row .value {{ font-weight:600; }}
        table {{ width:100%; border-collapse:collapse; margin:16px 0; }}
        th, td {{ border:1px solid #ddd; padding:8px; font-size:13px; }}
        th {{ background:#f5f5f5; font-weight:600; }}
        .section-title {{ font-size:15px; font-weight:700; margin:20px 0 8px; padding-bottom:4px; border-bottom:1px solid #eee; }}
        .summary {{ display:grid; grid-template-columns:1fr 1fr; gap:8px; margin:16px 0; }}
        .summary .box {{ border:1px solid #ddd; border-radius:6px; padding:10px; text-align:center; }}
        .summary .box .lbl {{ font-size:12px; color:#888; display:block; }}
        .summary .box .val {{ font-size:16px; font-weight:700; }}
        .text-success {{ color:#198754; }} .text-danger {{ color:#dc3545; }}
        .badge {{ display:inline-block; padding:3px 12px; border-radius:4px; font-size:13px; font-weight:600; color:#fff; }}
        .bg-issued {{ background:#0d6efd; }} .bg-paid {{ background:#198754; }} .bg-cancelled {{ background:#dc3545; }}
    </style>
</head>
<body>
    <div class='header'>
        <h1>فاتورة</h1>
        <div class='meta'>رقم الفاتورة: {inv.InvoiceNumber ?? "—"}</div>
        <div class='meta'>التاريخ: {inv.CreatedAtUtc:yyyy-MM-dd}</div>
        <div style='margin-top:8px'>
            <span class='badge {(inv.Status == "Issued" ? "bg-issued" : inv.Status == "Paid" ? "bg-paid" : "bg-cancelled")}'>
                {(inv.Status == "Issued" ? "صادرة" : inv.Status == "Paid" ? "مدفوعة" : "ملغاة")}
            </span>
        </div>
    </div>

    <div class='info-row'><span class='label'>اسم العميل</span><span class='value'>{inv.ClientNameAr}</span></div>
    <div class='info-row'><span class='label'>رقم العميل</span><span class='value'>{inv.ClientPhone}</span></div>
    <div class='info-row'><span class='label'>الفرع</span><span class='value'>{inv.BranchNameAr}</span></div>

    <div class='section-title'>البنود</div>
    <table>
        <thead><tr>
            <th style='width:36px'>#</th>
            <th>الوصف</th>
            <th>الكمية</th>
            <th>سعر الوحدة</th>
            <th>الإجمالي</th>
        </tr></thead>
        <tbody>{itemsRows}</tbody>
    </table>

    <div class='summary'>
        <div class='box'><span class='lbl'>المجموع الفرعي</span><div class='val'>{inv.SubTotal:F2}</div></div>
        <div class='box'><span class='lbl'>الخصم</span><div class='val text-danger'>{inv.DiscountAmount:F2}</div></div>
        <div class='box'><span class='lbl'>الضريبة ({inv.TaxRate * 100:F0}%)</span><div class='val'>{inv.TaxAmount:F2}</div></div>
        <div class='box' style='background:#f5f5f5'><span class='lbl'>الإجمالي شامل الضريبة</span><div class='val'>{inv.TotalWithTax:F2} {inv.Currency}</div></div>
        <div class='box'><span class='lbl'>المدفوع</span><div class='val text-success'>{inv.TotalPaid:F2}</div></div>
        <div class='box'><span class='lbl'>الرصيد</span><div class='val {(inv.Balance > 0 ? "text-danger" : "text-success")}'>{inv.Balance:F2} {inv.Currency}</div></div>
    </div>

    {(inv.Payments.Count > 0 ? $@"
    <div class='section-title'>المدفوعات</div>
    <table>
        <thead><tr>
            <th style='width:36px'>#</th>
            <th>التاريخ</th>
            <th>النوع</th>
            <th>المبلغ</th>
            <th>الطريقة</th>
        </tr></thead>
        <tbody>{paymentsRows}</tbody>
    </table>" : "")}

</body>
</html>";

        return Content(html, "text/html");
    }
}
