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

            // -- 4. Replace items if provided ----------------------------------------
            if (req.Items is { Count: > 0 })
            {
                foreach (var item in invoice.Items.ToList())
                    invoice.RemoveItem(item.Id);

                var serviceIds = req.Items
                    .Where(i => i.ServiceId.HasValue && i.ServiceId.Value != Guid.Empty)
                    .Select(i => i.ServiceId!.Value)
                    .Distinct()
                    .ToList();

                var services = serviceIds.Count > 0
                    ? await serviceRepo.Query()
                        .Where(s => serviceIds.Contains(s.Id))
                        .ToDictionaryAsync(s => s.Id, ct)
                    : new Dictionary<Guid, Service>();

                foreach (var item in req.Items)
                {
                    string? serviceNameAr = null;
                    string? serviceNameEn = null;

                    if (item.ServiceId.HasValue
                        && item.ServiceId.Value != Guid.Empty
                        && services.TryGetValue(item.ServiceId.Value, out var service))
                    {
                        serviceNameAr = service.NameAr;
                        serviceNameEn = service.NameEn;
                    }

                    invoice.AddItem(
                        item.Description,
                        Money.Create(item.UnitPrice),
                        item.ServiceId,
                        serviceNameAr,
                        serviceNameEn);
                }
            }

            // -- 5. Apply discount ----------------------------------------------------
            if (req.Discount.HasValue)
                invoice.SetDiscount(Money.Create(req.Discount.Value));

            await unitOfWork.SaveChangesAsync(ct);
            return Ok(invoice.Id);
        }
        catch (Exception ex)
        {
            return Fail($"Failed to update invoice: {ex.Message}");
        }
    }
}
