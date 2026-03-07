using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class InvoiceItem : Entity
{
    public string Description { get; private set; } = string.Empty;
    public Money UnitPrice { get; private set; } = Money.Zero();
    public Money TotalPrice { get; private set; } = Money.Zero();

    // Optional link to a service
    public Guid? ServiceId { get; private set; }
    public string? ServiceNameAr { get; private set; }
    public string? ServiceNameEn { get; private set; }
    public decimal AdjustmentAmount { get; private set; }

    private InvoiceItem() { } // EF

    internal InvoiceItem(string description, Money unitPrice,
        Guid? serviceId = null, string? serviceNameAr = null, string? serviceNameEn = null,
        decimal adjustmentAmount = 0)
    {
        Description      = description;
        UnitPrice        = unitPrice;
        TotalPrice       = adjustmentAmount != 0
            ? Money.CreateAllowNegative(adjustmentAmount, unitPrice.Currency)
            : Money.CreateAllowNegative(unitPrice.Amount, unitPrice.Currency);
        ServiceId        = serviceId;
        ServiceNameAr    = serviceNameAr;
        ServiceNameEn    = serviceNameEn;
        AdjustmentAmount = adjustmentAmount;
    }

    internal void Update(string description, Money unitPrice, decimal adjustmentAmount = 0)
    {
        Description      = description;
        UnitPrice        = unitPrice;
        TotalPrice       = adjustmentAmount != 0
            ? Money.CreateAllowNegative(adjustmentAmount, unitPrice.Currency)
            : Money.CreateAllowNegative(unitPrice.Amount, unitPrice.Currency);
        AdjustmentAmount = adjustmentAmount;
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
