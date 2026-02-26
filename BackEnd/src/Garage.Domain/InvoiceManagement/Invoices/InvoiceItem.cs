using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class InvoiceItem : Entity
{
    public string Description { get; private set; } = string.Empty;
    public int Quantity { get; private set; } = 1;
    public Money UnitPrice { get; private set; } = Money.Zero();
    public Money TotalPrice { get; private set; } = Money.Zero();

    // Optional link to a service
    public Guid? ServiceId { get; private set; }
    public string? ServiceNameAr { get; private set; }
    public string? ServiceNameEn { get; private set; }

    private InvoiceItem() { } // EF

    internal InvoiceItem(string description, int quantity, Money unitPrice,
        Guid? serviceId = null, string? serviceNameAr = null, string? serviceNameEn = null)
    {
        Description    = description;
        Quantity       = quantity;
        UnitPrice      = unitPrice;
        TotalPrice     = Money.CreateAllowNegative(unitPrice.Amount * quantity, unitPrice.Currency);
        ServiceId      = serviceId;
        ServiceNameAr  = serviceNameAr;
        ServiceNameEn  = serviceNameEn;
    }

    internal void Update(string description, int quantity, Money unitPrice)
    {
        Description = description;
        Quantity    = quantity;
        UnitPrice   = unitPrice;
        TotalPrice  = Money.CreateAllowNegative(unitPrice.Amount * quantity, unitPrice.Currency);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
