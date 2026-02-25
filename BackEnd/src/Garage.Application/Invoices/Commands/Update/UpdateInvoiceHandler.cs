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

namespace Garage.Application.Invoices.Commands.Update;

public sealed class UpdateInvoiceHandler(
    IRepository<Invoice>     invoiceRepo,
    IReadRepository<Client>  clientRepo,
    IReadRepository<Branch>  branchRepo,
    IReadRepository<Service> serviceRepo,
    IUnitOfWork              unitOfWork)
    : BaseCommandHandler<UpdateInvoiceCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(UpdateInvoiceCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // -- 1. Load invoice ------------------------------------------------------
        var invoice = await invoiceRepo.QueryTracking()
            .Include(i => i.Items)
            .FirstOrDefaultAsync(i => i.Id == command.Id, ct);

        if (invoice is null) return Fail("Invoice not found.");

        // -- 2. Validate lookups --------------------------------------------------
        var client = await clientRepo.Query()
            .FirstOrDefaultAsync(c => c.Id == req.ClientId, ct);
        if (client is null) return Fail("Client not found.");

        var branch = await branchRepo.GetByIdAsync(req.BranchId, ct);
        if (branch is null) return Fail("Branch not found.");

        // -- 3. Update invoice ----------------------------------------------------
        try
        {
            invoice.Update(req.Notes, req.DueDate);

            invoice.UpdateClientSnapshot(new ClientReference(
                ClientId:    client.Id,
                NameAr:      client.NameAr,
                NameEn:      client.NameEn,
                PhoneNumber: client.PhoneNumber,
                Email:       null));

            invoice.UpdateBranchSnapshot(new BranchReference(
                BranchId: branch.Id,
                NameAr:   branch.NameAr,
                NameEn:   branch.NameEn));

            // -- 4. Replace items if provided (Draft only) -------------------------
            if (req.Items is { Count: > 0 })
            {
                foreach (var item in invoice.Items.ToList())
                    invoice.RemoveItem(item.Id);

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
            }

            await unitOfWork.SaveChangesAsync(ct);
            return Ok(invoice.Id);
        }
        catch (Exception ex)
        {
            return Fail($"Failed to update invoice: {ex.Message}");
        }
    }
}
