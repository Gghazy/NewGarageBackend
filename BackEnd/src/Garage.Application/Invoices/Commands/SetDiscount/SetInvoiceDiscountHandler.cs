using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.SetDiscount;

public sealed class SetInvoiceDiscountHandler(
    IRepository<Invoice> repo,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<SetInvoiceDiscountCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(SetInvoiceDiscountCommand command, CancellationToken ct)
    {
        var invoice = await repo.QueryTracking()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == command.InvoiceId, ct);

        if (invoice is null)
            return Fail("Invoice not found.");

        try
        {
            invoice.SetDiscount(Money.Create(command.Amount));
            await unitOfWork.SaveChangesAsync(ct);
            return Ok(invoice.Id);
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }
    }
}
