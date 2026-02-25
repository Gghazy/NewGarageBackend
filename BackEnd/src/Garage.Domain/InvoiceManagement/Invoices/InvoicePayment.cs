using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Shared;
using Domain.ExaminationManagement.Examinations;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class InvoicePayment : Entity
{
    public Money Amount { get; private set; } = Money.Zero();
    public PaymentMethod Method { get; private set; }
    public PaymentType Type { get; private set; }
    public string? Notes { get; private set; }

    private InvoicePayment() { } // EF

    internal InvoicePayment(Money amount, PaymentMethod method, PaymentType type, string? notes)
    {
        Amount = amount;
        Method = method;
        Type   = type;
        Notes  = Normalize(notes);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
