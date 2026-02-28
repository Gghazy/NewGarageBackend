using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Common;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;

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
            .Select(InvoiceProjection.ToDto);

        return await query.ToQueryResult(
            search.CurrentPage,
            search.ItemsPerPage,
            ct: ct);
    }
}
