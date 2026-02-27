using System.Linq.Expressions;
using Garage.Contracts.Invoices;
using Domain.ExaminationManagement.Examinations;
using Garage.Domain.InvoiceManagement.Invoices;

namespace Garage.Application.Invoices.Queries;

public static class InvoiceProjection
{
    public static readonly Expression<Func<Invoice, InvoiceDto>> ToDto =
        i => new InvoiceDto(
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
                p.MethodId,
                p.MethodNameAr,
                p.MethodNameEn,
                p.Type.ToString(),
                p.Notes,
                p.CreatedAtUtc
            )).ToList(),
            // Related invoices (populated separately in GetById)
            new List<RelatedInvoiceDto>(),
            // CreatedAt
            i.CreatedAtUtc
        );

    public static readonly Expression<Func<Invoice, RelatedInvoiceDto>> ToRelatedDto =
        r => new RelatedInvoiceDto(
            r.Id,
            r.InvoiceNumber,
            r.Type.ToString(),
            r.Status.ToString(),
            r.TotalWithTax.Amount,
            r.TotalPrice.Currency,
            r.CreatedAtUtc
        );
}
