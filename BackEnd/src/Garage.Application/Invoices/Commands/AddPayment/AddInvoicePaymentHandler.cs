using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Garage.Domain.PaymentMethods.Entity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.AddPayment;

public sealed class AddInvoicePaymentHandler(
    IRepository<Invoice> repo,
    ILookupRepository<PaymentMethodLookup> methodRepo,
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

        var method = await methodRepo.GetByIdAsync(req.MethodId, ct);
        if (method is null)
            return Fail("Payment method not found.");

        var currency = string.IsNullOrWhiteSpace(req.Currency) ? "SAR" : req.Currency;
        var amount   = Money.Create(req.Amount, currency);

        try
        {
            invoice.AddPayment(amount, method.Id, method.NameAr, method.NameEn, req.Notes);
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
