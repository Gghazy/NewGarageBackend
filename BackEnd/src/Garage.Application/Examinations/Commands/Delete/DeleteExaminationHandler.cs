using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Invoices;
using Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using PaymentType = Domain.ExaminationManagement.Examinations.PaymentType;
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

        // Load all invoices for this examination (with payments)
        var allInvoices = await invoiceRepo.QueryTracking()
            .Include(i => i.Items)
            .Include(i => i.Payments)
            .Where(i => i.ExaminationId == request.Id)
            .ToListAsync(ct);

        // Calculate net subtotal (before tax) to avoid double-taxation
        var invoiceSubTotal = allInvoices
            .Where(i => i.Type == InvoiceType.Invoice)
            .Sum(i => i.TotalPrice.Amount - i.DiscountAmount.Amount);

        var refundSubTotal = allInvoices
            .Where(i => i.Type == InvoiceType.Refund)
            .Sum(i => i.TotalPrice.Amount - i.DiscountAmount.Amount);

        var netSubTotal = invoiceSubTotal - refundSubTotal;

        // If customer is owed money, create a single refund invoice
        if (netSubTotal > 0)
        {
            var sourceInvoice = allInvoices.First(i => i.Type == InvoiceType.Invoice);
            var currency = sourceInvoice.TotalPrice.Currency;

            var refund = Invoice.CreateRefundInvoice(sourceInvoice, Money.Create(netSubTotal, currency));

            var refNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Refund, ct);
            refund.SetInvoiceNumber(refNumber);
            await invoiceRepo.AddAsync(refund, ct);
        }

        // Cancel/delete all existing invoices
        foreach (var invoice in allInvoices)
        {
            if (invoice.Status == InvoiceStatus.Paid)
                invoice.ForceCancel("Examination deleted");
            else
                await invoiceRepo.SoftDeleteAsync(invoice, ct: ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
