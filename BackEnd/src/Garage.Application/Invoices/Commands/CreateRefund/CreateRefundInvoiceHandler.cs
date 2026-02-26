using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Application.Invoices.Commands.CreateRefund;

public sealed class CreateRefundInvoiceHandler(
    IRepository<Invoice> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<CreateRefundInvoiceCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateRefundInvoiceCommand command, CancellationToken ct)
    {
        var original = await repo.QueryTracking()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == command.OriginalInvoiceId, ct);

        if (original is null)
            return Fail("Invoice not found.");

        try
        {
            // Refund invoices are now created automatically when adding a refund payment.
            // This handler creates a standalone refund invoice for the full amount.
            var refund = Invoice.CreateRefundInvoice(original, original.TotalWithTax);
            await repo.AddAsync(refund, ct);
            await unitOfWork.SaveChangesAsync(ct);
            return Ok(refund.Id);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
