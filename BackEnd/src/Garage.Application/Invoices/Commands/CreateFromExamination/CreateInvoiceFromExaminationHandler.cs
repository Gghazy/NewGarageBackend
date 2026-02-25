using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.CreateFromExamination;

public sealed class CreateInvoiceFromExaminationHandler(
    IRepository<Invoice>        invoiceRepo,
    IReadRepository<Examination> examRepo,
    IUnitOfWork                  unitOfWork)
    : BaseCommandHandler<CreateInvoiceFromExaminationCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateInvoiceFromExaminationCommand command, CancellationToken ct)
    {
        // -- 1. Load examination with items ------------------------------------------

        var exam = await examRepo.Query()
            .Include(e => e.Items)
            .FirstOrDefaultAsync(e => e.Id == command.ExaminationId, ct);

        if (exam is null)
            return Fail("Examination not found.");

        // -- 2. Check duplicate ------------------------------------------------------

        var alreadyExists = await invoiceRepo.Query()
            .AnyAsync(i => i.ExaminationId == command.ExaminationId, ct);

        if (alreadyExists)
            return Fail("An invoice has already been created for this examination.");

        // -- 3. Create invoice -------------------------------------------------------

        var invoice = Invoice.Create(
            exam.Client,
            exam.Branch,
            "EGP",
            examinationId: exam.Id);

        if (!string.IsNullOrWhiteSpace(exam.Notes))
            invoice.SetNotes(exam.Notes);

        // -- 4. Copy examination items as invoice items ------------------------------

        foreach (var examItem in exam.Items)
        {
            invoice.AddItem(
                description:   examItem.Service.NameEn,
                quantity:      1,
                unitPrice:     examItem.Price,
                serviceId:     examItem.Service.ServiceId,
                serviceNameAr: examItem.Service.NameAr,
                serviceNameEn: examItem.Service.NameEn);
        }

        await invoiceRepo.AddAsync(invoice, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(invoice.Id);
    }
}
