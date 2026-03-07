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
        return await repo.Query()
            .Where(i => i.Id == request.Id)
            .Select(InvoiceProjection.ToDto)
            .FirstOrDefaultAsync(ct);
    }
}
