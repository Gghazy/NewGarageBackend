using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Common;
using Garage.Contracts.Invoices;
using Garage.Domain.InvoiceManagement.Invoices;

namespace Garage.Application.Invoices.Queries.GetAll;

public sealed class GetAllInvoicesQueryHandler(IReadRepository<Invoice> repo)
    : BaseQueryHandler<GetAllInvoicesQuery, QueryResult<InvoiceDto>>
{
    public override async Task<QueryResult<InvoiceDto>> Handle(GetAllInvoicesQuery request, CancellationToken ct)
    {
        var search = request.Search;

        var query = repo.Query()
            .Where(i => i.Type == InvoiceType.Invoice)
            .WhereIf(
                !string.IsNullOrWhiteSpace(search.TextSearch),
                i => i.Client.NameAr.Contains(search.TextSearch!)   ||
                     i.Client.NameEn.Contains(search.TextSearch!)   ||
                     i.Client.PhoneNumber.Contains(search.TextSearch!) ||
                     (i.InvoiceNumber != null && i.InvoiceNumber.Contains(search.TextSearch!)))
            .Select(i => new InvoiceDto(
                i.Id,
                i.InvoiceNumber,
                i.ExaminationId,
                i.OriginalInvoiceId,
                // Type & Status
                i.Type.ToString(),
                i.Status.ToString(),
                // Client
                i.Client.ClientId,
                i.Client.NameAr,
                i.Client.NameEn,
                i.Client.PhoneNumber,
                // Branch
                i.Branch.BranchId,
                i.Branch.NameAr,
                i.Branch.NameEn,
                // Meta
                i.Notes,
                i.DueDate,
                // Financials
                i.TotalPrice.Amount,
                i.DiscountAmount.Amount,
                i.TaxRate,
                i.TaxAmount.Amount,
                i.TotalWithTax.Amount,
                i.TotalPrice.Currency,
                i.Payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount),
                i.Payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount),
                i.TotalWithTax.Amount
                    - i.Payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount)
                    + i.Payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount),
                // Items
                i.Items.Select(item => new InvoiceItemDto(
                    item.Id,
                    item.Description,
                    item.Quantity,
                    item.UnitPrice.Amount,
                    item.TotalPrice.Amount,
                    item.UnitPrice.Currency,
                    item.ServiceId,
                    item.ServiceNameAr,
                    item.ServiceNameEn
                )).ToList(),
                // Payments
                i.Payments.Select(p => new InvoicePaymentDto(
                    p.Id,
                    p.Amount.Amount,
                    p.Amount.Currency,
                    p.Method.ToString(),
                    p.Type.ToString(),
                    p.Notes,
                    p.CreatedAtUtc
                )).ToList(),
                // Related invoices (not needed in list view)
                new List<RelatedInvoiceDto>(),
                // CreatedAt
                i.CreatedAtUtc
            ));

        return await query.ToQueryResult(
            search.CurrentPage,
            search.ItemsPerPage,
            ct: ct);
    }
}
