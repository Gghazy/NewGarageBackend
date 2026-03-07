using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Invoices;
using Domain.ExaminationManagement.Examinations;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Commands.Delete;

public class DeleteExaminationHandler(
    IRepository<Examination> repository,
    IRepository<Invoice> invoiceRepo,
    IUnitOfWork unitOfWork,
    InvoiceNumberGenerator invoiceNumberGenerator)
    : BaseCommandHandler<DeleteExaminationCommand, bool>
{
    public override async Task<Result<bool>> Handle(DeleteExaminationCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        entity.MarkDeleted();
        await repository.SoftDeleteAsync(entity, ct: ct);

        // Handle related invoices
        var invoices = await invoiceRepo.QueryTracking()
            .Include(i => i.Items)
            .Where(i => i.ExaminationId == request.Id
                     && i.Type == InvoiceType.Invoice)
            .ToListAsync(ct);

        foreach (var invoice in invoices)
        {
            if (invoice.Status == InvoiceStatus.Paid)
            {
                // Create refund invoice, keep original as-is
                var refund = Invoice.CreateEmptyRefundInvoice(invoice);
                foreach (var item in invoice.Items)
                    refund.AddItem(item.Description, item.TotalPrice, item.ServiceId, item.ServiceNameAr, item.ServiceNameEn, item.TotalPrice.Amount);

                refund.MarkAsRefunded();

                var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
                refund.SetInvoiceNumber(refNumber);
                await invoiceRepo.AddAsync(refund, ct);
            }
            else
            {
                await invoiceRepo.SoftDeleteAsync(invoice, ct: ct);
            }
        }

        // Soft-delete any remaining invoices (Refund, Adjustment, etc.)
        var otherInvoices = await invoiceRepo.QueryTracking()
            .Where(i => i.ExaminationId == request.Id)
            .ToListAsync(ct);

        foreach (var invoice in otherInvoices)
            await invoiceRepo.SoftDeleteAsync(invoice, ct: ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
