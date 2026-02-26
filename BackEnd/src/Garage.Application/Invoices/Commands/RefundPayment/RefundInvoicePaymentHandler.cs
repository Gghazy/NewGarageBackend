using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.RefundPayment;

public sealed class RefundInvoicePaymentHandler(
    IRepository<Invoice> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<RefundInvoicePaymentCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(RefundInvoicePaymentCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var invoice = await repo.QueryTracking()
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == command.InvoiceId, ct);

        if (invoice is null)
            return Fail("Invoice not found.");

        if (string.IsNullOrWhiteSpace(req.Method))
            return Fail("Payment method is required.");

        var currency = string.IsNullOrWhiteSpace(req.Currency) ? "SAR" : req.Currency;
        var amount   = Money.Create(req.Amount, currency);

        try
        {
            // Record refund transaction on the original invoice
            invoice.AddRefund(amount, req.Method.Trim(), req.Notes);

            // Create a refund invoice for documentation
            var refundInvoice = Invoice.CreateRefundInvoice(invoice, amount);
            await repo.AddAsync(refundInvoice, ct);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await unitOfWork.SaveChangesAsync(ct);

        var payment = invoice.Payments.Last();
        return Ok(payment.Id);
    }
}
