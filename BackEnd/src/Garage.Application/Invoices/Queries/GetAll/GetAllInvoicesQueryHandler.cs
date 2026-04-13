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

        // Parse optional filters
        InvoiceType? typeFilter = null;
        if (!string.IsNullOrWhiteSpace(search.InvoiceType)
            && Enum.TryParse<InvoiceType>(search.InvoiceType, true, out var parsedType))
            typeFilter = parsedType;

        InvoiceStatus? statusFilter = null;
        if (!string.IsNullOrWhiteSpace(search.Status)
            && Enum.TryParse<InvoiceStatus>(search.Status, true, out var parsedStatus))
            statusFilter = parsedStatus;

        var query = repo.Query()
            .WhereIf(typeFilter.HasValue, i => i.Type == typeFilter!.Value)
            .WhereIf(!typeFilter.HasValue && !statusFilter.HasValue, i => i.Type == InvoiceType.Invoice)
            .WhereIf(statusFilter.HasValue, i => i.Status == statusFilter!.Value)
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

        // Calculate net totals by subtracting refund invoice amounts
        var invoiceIds = result.Items.Select(i => i.Id).ToList();
        if (invoiceIds.Count > 0)
        {
            var refundTotals = await repo.Query()
                .Where(i => i.OriginalInvoiceId.HasValue
                         && invoiceIds.Contains(i.OriginalInvoiceId!.Value)
                         && i.Type == InvoiceType.Refund)
                .GroupBy(i => i.OriginalInvoiceId!.Value)
                .Select(g => new { OriginalId = g.Key, Total = g.Sum(x => x.TotalWithTax.Amount) })
                .ToDictionaryAsync(x => x.OriginalId, x => x.Total, ct);

            if (refundTotals.Count > 0)
            {
                result.AddItems(result.Items.Select(inv =>
                    refundTotals.TryGetValue(inv.Id, out var refunded)
                        ? inv with { NetTotal = inv.TotalWithTax - refunded }
                        : inv));
            }
        }

        return result;
    }
}
