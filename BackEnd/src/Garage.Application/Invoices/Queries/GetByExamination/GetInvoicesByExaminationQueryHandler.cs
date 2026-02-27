using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetByExamination;

public sealed class GetInvoicesByExaminationQueryHandler(IReadRepository<Invoice> repo)
    : BaseQueryHandler<GetInvoicesByExaminationQuery, List<InvoiceDto>>
{
    public override async Task<List<InvoiceDto>> Handle(GetInvoicesByExaminationQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Where(i => i.ExaminationId == request.ExaminationId)
            .Select(InvoiceProjection.ToDto)
            .OrderByDescending(i => i.CreatedAtUtc)
            .ToListAsync(ct);
    }
}
