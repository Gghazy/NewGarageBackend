using Domain.ExaminationManagement.Examinations;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class InvoicePayment : Entity
{
    public Money Amount { get; private set; } = Money.Zero();
    public string Method { get; private set; } = null!;
    public PaymentType Type { get; private set; }
    public string? Notes { get; private set; }

    private InvoicePayment() { } // EF

    internal InvoicePayment(Money amount, string method, PaymentType type, string? notes)
    {
        Amount = amount;
        Method = method;
        Type   = type;
        Notes  = Normalize(notes);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
