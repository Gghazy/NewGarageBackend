using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Shared;

namespace Domain.ExaminationManagement.Examinations;

public sealed class Payment : Entity
{
    public Money Amount { get; private set; } = Money.Zero();
    public PaymentMethod Method { get; private set; }
    public string? Notes { get; private set; }

    private Payment() { } // EF

    internal Payment(Money amount, PaymentMethod method, string? notes)
    {
        Amount = amount;
        Method = method;
        Notes  = Normalize(notes);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
