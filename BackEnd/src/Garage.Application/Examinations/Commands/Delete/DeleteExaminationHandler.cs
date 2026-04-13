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

        // Soft-delete ALL existing invoices (won't appear in reports or revenue)
        foreach (var invoice in allInvoices)
            await invoiceRepo.SoftDeleteAsync(invoice, ct: ct);

        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
