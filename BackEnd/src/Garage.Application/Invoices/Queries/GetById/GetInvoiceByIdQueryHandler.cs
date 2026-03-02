using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetById;

public sealed class GetInvoiceByIdQueryHandler(IReadRepository<Invoice> repo)
    : BaseQueryHandler<GetInvoiceByIdQuery, InvoiceDto?>
{
    public override async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        var invoice = await repo.Query()
            .Where(i => i.Id == request.Id)
            .Select(InvoiceProjection.ToDto)
            .FirstOrDefaultAsync(ct);

        if (invoice is null) return null;

        // Determine the root (parent) invoice ID for the chain
        var rootId = invoice.OriginalInvoiceId ?? invoice.Id;

        // Fetch all invoices in the chain (parent + children) including the current one
        var relatedInvoices = await repo.Query()
            .Where(r => r.Id == rootId || r.OriginalInvoiceId == rootId)
            .OrderBy(r => r.CreatedAtUtc)
            .Select(InvoiceProjection.ToRelatedDto)
            .ToListAsync(ct);

        return invoice with { RelatedInvoices = relatedInvoices };
    }
}
