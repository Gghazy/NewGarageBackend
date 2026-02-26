using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Application.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Commands.CreateFromExamination;

public sealed class CreateInvoiceFromExaminationHandler(
    IRepository<Invoice>         invoiceRepo,
    IReadRepository<Examination> examRepo,
    IUnitOfWork                  unitOfWork,
    ExaminationService           examinationService,
    InvoiceNumberGenerator       invoiceNumberGenerator)
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
            .AnyAsync(i => i.ExaminationId == command.ExaminationId
                        && i.Type == InvoiceType.Invoice, ct);

        if (alreadyExists)
            return Fail("An invoice has already been created for this examination.");

        // -- 3. Create invoice -------------------------------------------------------

        var invoice = Invoice.Create(
            exam.Client,
            exam.Branch,
            "SAR",
            examinationId: exam.Id);

        if (!string.IsNullOrWhiteSpace(exam.Notes))
            invoice.SetNotes(exam.Notes);

        // -- 4. Lookup prices from ServicePrice and add invoice items ----------------

        var priceMap = await examinationService.LookupServicePricesAsync(
            exam.Items.Select(i => i.Service.ServiceId),
            exam.Vehicle.CarMarkId,
            exam.Vehicle.Year,
            ct);

        foreach (var examItem in exam.Items)
        {
            var unitPrice = priceMap.TryGetValue(examItem.Service.ServiceId, out var p)
                ? Money.Create(p) : Money.Zero();

            invoice.AddItem(
                description:   examItem.Service.NameEn,
                quantity:      examItem.Quantity,
                unitPrice:     unitPrice,
                serviceId:     examItem.Service.ServiceId,
                serviceNameAr: examItem.Service.NameAr,
                serviceNameEn: examItem.Service.NameEn);
        }

        // -- 5. Assign invoice number ---------------------------------------------------
        var invoiceNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Invoice, ct);
        invoice.SetInvoiceNumber(invoiceNumber);

        await invoiceRepo.AddAsync(invoice, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(invoice.Id);
    }
}
