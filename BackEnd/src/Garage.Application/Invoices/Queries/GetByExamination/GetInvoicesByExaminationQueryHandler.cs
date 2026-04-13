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
        var invoices = await repo.Query()
            .Where(i => i.ExaminationId == request.ExaminationId)
            .OrderByDescending(i => i.CreatedAtUtc)
            .Select(InvoiceProjection.ToDto)
            .ToListAsync(ct);

        var invoiceIds = invoices.Select(i => i.Id).ToList();
        var refundTotals = await repo.Query()
            .Where(i => i.OriginalInvoiceId.HasValue
                     && invoiceIds.Contains(i.OriginalInvoiceId!.Value)
                     && i.Type == InvoiceType.Refund)
            .GroupBy(i => i.OriginalInvoiceId!.Value)
            .Select(g => new { OriginalId = g.Key, Total = g.Sum(x => x.TotalWithTax.Amount) })
            .ToDictionaryAsync(x => x.OriginalId, x => x.Total, ct);

        if (refundTotals.Count > 0)
        {
            invoices = invoices.Select(inv =>
                refundTotals.TryGetValue(inv.Id, out var refunded)
                    ? inv with { NetTotal = inv.TotalWithTax - refunded }
                    : inv).ToList();
        }

        return invoices;
    }
}
