using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Branches.Entities;
using Garage.Domain.Clients.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Garage.Domain.InvoiceManagement.Invoices;
using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;

using Garage.Application.Invoices;

namespace Garage.Application.Invoices.Commands.Create;

public sealed class CreateInvoiceHandler(
    IRepository<Invoice>    invoiceRepo,
    IReadRepository<Client> clientRepo,
    IReadRepository<Branch> branchRepo,
    IReadRepository<Service> serviceRepo,
    IUnitOfWork             unitOfWork,
    InvoiceNumberGenerator  invoiceNumberGenerator)
    : BaseCommandHandler<CreateInvoiceCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateInvoiceCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // -- 1. Validate lookups --------------------------------------------------

        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == req.ClientId, ct);
        if (client is null) return Fail("Client not found.");

        var branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        if (req.Items is not { Count: > 0 })
            return Fail("At least one item is required.");

        // -- 2. Create invoice ----------------------------------------------------

        var clientRef = new ClientReference(
            ClientId:    client.Id,
            NameAr:      client.NameAr,
            NameEn:      client.NameEn,
            PhoneNumber: client.PhoneNumber,
            Email:       null);

        var branchRef = new BranchReference(
            BranchId: branch.Id,
            NameAr:   branch.NameAr,
            NameEn:   branch.NameEn);

        var invoice = Invoice.Create(clientRef, branchRef);

        if (!string.IsNullOrWhiteSpace(req.Notes))
            invoice.SetNotes(req.Notes);
        if (req.DueDate.HasValue)
            invoice.SetDueDate(req.DueDate);

        // -- 3. Add items ---------------------------------------------------------

        foreach (var item in req.Items)
        {
            string? serviceNameAr = null;
            string? serviceNameEn = null;

            if (item.ServiceId.HasValue && item.ServiceId.Value != Guid.Empty)
            {
                var service = await serviceRepo.GetByIdAsync(item.ServiceId.Value, ct);
                if (service is not null)
                {
                    serviceNameAr = service.NameAr;
                    serviceNameEn = service.NameEn;
                }
            }

            invoice.AddItem(
                item.Description,
                item.Quantity,
                Money.Create(item.UnitPrice),
                item.ServiceId,
                serviceNameAr,
                serviceNameEn);
        }

        // -- 4. Apply discount --------------------------------------------------------
        if (req.Discount.HasValue && req.Discount.Value > 0)
            invoice.SetDiscount(Money.Create(req.Discount.Value));

        // -- 5. Assign invoice number ------------------------------------------------
        var invoiceNumber = await invoiceNumberGenerator.GenerateAsync(InvoiceType.Invoice, ct);
        invoice.SetInvoiceNumber(invoiceNumber);

        await invoiceRepo.AddAsync(invoice, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(invoice.Id);
    }
}
