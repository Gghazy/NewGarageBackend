using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.CreateFromExamination;

public sealed class CreateInvoiceFromExaminationHandler(
    IReadRepository<Invoice>     invoiceRepo,
    IReadRepository<Examination> examRepo,
    InvoiceSyncService           invoiceSyncService)
    : BaseCommandHandler<CreateInvoiceFromExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateInvoiceFromExaminationCommand command, CancellationToken ct)
    {
        var exam = await examRepo.Query()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.ExaminationId, ct);

        if (exam is null)
            return Fail("Examination not found.");

        var alreadyExists = await invoiceRepo.Query()
            .AnyAsync(i => i.ExaminationId == command.ExaminationId
                        && i.Type == InvoiceType.Invoice, ct);

        if (alreadyExists)
            return Fail("An invoice has already been created for this examination.");

        var invoice = await invoiceSyncService.CreateInvoiceFromExaminationAsync(
            exam, exam.Client, exam.Branch, overridePrices: null, ct);

        return Ok(invoice.Id);
    }
}
