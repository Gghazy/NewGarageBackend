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

    // Per-item discount
    public decimal DiscountPercent { get; private set; }
    public Money DiscountAmount { get; private set; } = Money.Zero();

    private InvoiceItem() { } // EF

    internal InvoiceItem(string description, Money unitPrice,
        Guid? serviceId = null, string? serviceNameAr = null, string? serviceNameEn = null,
        decimal adjustmentAmount = 0, decimal discountPercent = 0)
    {
        Description      = description;
        UnitPrice        = unitPrice;
        ServiceId        = serviceId;
        ServiceNameAr    = serviceNameAr;
        ServiceNameEn    = serviceNameEn;
        AdjustmentAmount = adjustmentAmount;
        DiscountPercent  = discountPercent;
        Recalculate(unitPrice, adjustmentAmount, discountPercent);
    }

    internal void Update(string description, Money unitPrice, decimal adjustmentAmount = 0, decimal discountPercent = 0)
    {
        Description      = description;
        UnitPrice        = unitPrice;
        AdjustmentAmount = adjustmentAmount;
        DiscountPercent  = discountPercent;
        Recalculate(unitPrice, adjustmentAmount, discountPercent);
    }

    internal void SetDiscount(decimal discountPercent)
    {
        DiscountPercent = discountPercent;
        Recalculate(UnitPrice, AdjustmentAmount, discountPercent);
    }

    private void Recalculate(Money unitPrice, decimal adjustmentAmount, decimal discountPercent)
    {
        var baseAmount = adjustmentAmount != 0 ? adjustmentAmount : unitPrice.Amount;
        var discountAmt = discountPercent > 0
            ? Math.Round(baseAmount * discountPercent / 100m, 2)
            : 0m;
        DiscountAmount = Money.CreateAllowNegative(discountAmt, unitPrice.Currency);
        TotalPrice = Money.CreateAllowNegative(baseAmount - discountAmt, unitPrice.Currency);
    }
}
