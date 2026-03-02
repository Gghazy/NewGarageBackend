using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Common;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetAll;

public sealed class GetAllInvoicesQueryHandler(
    IReadRepository<Invoice> repo,
    IBranchAccessService branchAccess)
    : BaseQueryHandler<GetAllInvoicesQuery, QueryResult<InvoiceDto>>
{
    public override async Task<QueryResult<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken ct)
    {
        var search = request.Search;
        var branchIds = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = repo.Query()
            .Where(i => i.Type == InvoiceType.Invoice)
            .WhereBranchAccessible(branchIds)
            .WhereIf(
                !string.IsNullOrWhiteSpace(search.TextSearch),
                i => i.Client.NameAr.Contains(search.TextSearch!)   ||
                     i.Client.NameEn.Contains(search.TextSearch!)   ||
                     i.Client.PhoneNumber.Contains(search.TextSearch!) ||
                     (i.InvoiceNumber != null && i.InvoiceNumber.Contains(search.TextSearch!)))
            .WhereIf(search.DateFrom.HasValue,
                i => i.CreatedAtUtc >= search.DateFrom!.Value.Date)
            .WhereIf(search.DateTo.HasValue,
                i => i.CreatedAtUtc < search.DateTo!.Value.Date.AddDays(1))
            .WhereIf(search.BranchId.HasValue,
                i => i.Branch.BranchId == search.BranchId!.Value)
            .Select(InvoiceProjection.ToDto);

        var result = await query.ToQueryResult(
            search.CurrentPage,
            search.ItemsPerPage,
            ct: ct);

        // Adjust TotalWithTax and Balance for invoices that have refund children
        var parentIds = result.Items.Select(i => i.Id).ToList();
        if (parentIds.Count > 0)
        {
            var refundTotals = await repo.Query()
                .Where(i => i.OriginalInvoiceId != null
                         && parentIds.Contains(i.OriginalInvoiceId!.Value)
                         && i.Type == InvoiceType.Refund
                         && i.Status != InvoiceStatus.Cancelled)
                .GroupBy(i => i.OriginalInvoiceId!.Value)
                .Select(g => new
                {
                    ParentId = g.Key,
                    RefundTotal = g.Sum(i => i.TotalWithTax.Amount)
                })
                .ToListAsync(ct);

            if (refundTotals.Count > 0)
            {
                var refundMap = refundTotals.ToDictionary(r => r.ParentId, r => r.RefundTotal);
                var updatedItems = result.Items.Select(item =>
                {
                    if (refundMap.TryGetValue(item.Id, out var refundTotal))
                    {
                        var netTotal = item.TotalWithTax - refundTotal;
                        var netBalance = netTotal - item.TotalPaid;
                        return item with { TotalWithTax = netTotal, Balance = netBalance };
                    }
                    return item;
                }).ToList();

                result.AddItems(updatedItems);
            }
        }

        return result;
    }
}
