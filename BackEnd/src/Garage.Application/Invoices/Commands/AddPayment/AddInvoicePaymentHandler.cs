using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.AddPayment;

public sealed class AddInvoicePaymentHandler(
    IRepository<Invoice> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<AddInvoicePaymentCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(AddInvoicePaymentCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var invoice = await repo.QueryTracking()
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == command.InvoiceId, ct);

        if (invoice is null)
            return Fail("Invoice not found.");

        if (!Enum.TryParse<PaymentMethod>(req.Method, ignoreCase: true, out var method))
            return Fail($"Invalid payment method '{req.Method}'. Use Cash, Card, BankTransfer or Cheque.");

        var currency = string.IsNullOrWhiteSpace(req.Currency) ? "SAR" : req.Currency;
        var amount   = Money.Create(req.Amount, currency);

        try
        {
            invoice.AddPayment(amount, method, req.Notes);
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
