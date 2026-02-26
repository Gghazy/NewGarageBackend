using Domain.ExaminationManagement.Examinations;
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
                // Related invoices – populated below
                new List<RelatedInvoiceDto>(),
                // CreatedAt
                i.CreatedAtUtc
            ))
            .FirstOrDefaultAsync(ct);

        if (invoice is null) return null;

        // Fetch related invoices (Refund / Adjustment that reference this invoice)
        var relatedInvoices = await repo.Query()
            .Where(r => r.OriginalInvoiceId == request.Id)
            .Select(r => new RelatedInvoiceDto(
                r.Id,
                r.InvoiceNumber,
                r.Type.ToString(),
                r.Status.ToString(),
                r.TotalWithTax.Amount,
                r.TotalPrice.Currency,
                r.CreatedAtUtc
            ))
            .ToListAsync(ct);

        return invoice with { RelatedInvoices = relatedInvoices };
    }
}
