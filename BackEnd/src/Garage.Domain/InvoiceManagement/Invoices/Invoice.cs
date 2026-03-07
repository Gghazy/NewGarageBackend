using Domain.ExaminationManagement.Examinations;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class Invoice : AggregateRoot
{
    public ClientReference Client { get; private set; } = default!;
    public BranchReference Branch { get; private set; } = default!;

    public InvoiceType Type { get; private set; }
    public InvoiceStatus Status { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public Guid? ExaminationId { get; private set; }
    public Guid? OriginalInvoiceId { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Money TotalPrice    { get; private set; } = Money.Zero("SAR");
    public Money DiscountAmount { get; private set; } = Money.Zero("SAR");
    public decimal TaxRate     { get; private set; } = 0.15m;
    public Money TaxAmount     { get; private set; } = Money.Zero("SAR");
    public Money TotalWithTax  { get; private set; } = Money.Zero("SAR");

    private readonly List<InvoiceItem> _items = new();
    public IReadOnlyCollection<InvoiceItem> Items => _items.AsReadOnly();

    private readonly List<InvoicePayment> _payments = new();
    public IReadOnlyCollection<InvoicePayment> Payments => _payments.AsReadOnly();

    private readonly List<InvoiceHistory> _history = new();
    public IReadOnlyCollection<InvoiceHistory> History => _history.AsReadOnly();

    public decimal TotalPaid => _payments
        .Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount);
    public decimal TotalRefunded => _payments
        .Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount);
    public decimal Balance => TotalWithTax.Amount - TotalPaid + TotalRefunded;

    private Invoice() { }

    private Invoice(
        ClientReference client,
        BranchReference branch,
        string currency)
    {
        Client = client;
        Branch = branch;

        Type           = InvoiceType.Invoice;
        Status         = InvoiceStatus.Issued;
        TotalPrice     = Money.Zero(currency);
        DiscountAmount = Money.Zero(currency);
        TaxAmount      = Money.Zero(currency);
        TotalWithTax   = Money.Zero(currency);
    }

    public static Invoice Create(
        ClientReference client,
        BranchReference branch,
        string currency = "SAR",
        Guid? examinationId = null)
    {
        if (client.ClientId == Guid.Empty)  throw new DomainException("Client is required.");
        if (branch.BranchId == Guid.Empty)  throw new DomainException("Branch is required.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");

        var invoice = new Invoice(client, branch, currency);
        invoice.ExaminationId = examinationId;
        invoice.AddHistory(InvoiceHistoryAction.Created);
        return invoice;
    }

    public static Invoice CreateRefundInvoice(Invoice original, Money refundAmount)
    {
        if (original.Type == InvoiceType.Refund)
            throw new DomainException("Cannot create refund from a refund invoice.");

        var client = new ClientReference(
            original.Client.ClientId,
            original.Client.NameAr,
            original.Client.NameEn,
            original.Client.PhoneNumber,
            original.Client.Email);

        var branch = new BranchReference(
            original.Branch.BranchId,
            original.Branch.NameAr,
            original.Branch.NameEn);

        var refund = new Invoice(client, branch, refundAmount.Currency);
        refund.Type = InvoiceType.Refund;
        refund.OriginalInvoiceId = original.Id;
        refund.ExaminationId = original.ExaminationId;
        refund.AddItem("Refund", refundAmount);
        refund.Status = InvoiceStatus.Refunded;
        refund.AddHistory(InvoiceHistoryAction.RefundInvoiceCreated);

        return refund;
    }

    /// <summary>
    /// Creates a Refund invoice in Draft status (no items). Caller adds items then calls Issue().
    /// </summary>
    public static Invoice CreateEmptyRefundInvoice(Invoice original)
    {
        if (original.Type == InvoiceType.Refund)
            throw new DomainException("Cannot create refund from a refund invoice.");

        var client = new ClientReference(
            original.Client.ClientId,
            original.Client.NameAr,
            original.Client.NameEn,
            original.Client.PhoneNumber,
            original.Client.Email);

        var branch = new BranchReference(
            original.Branch.BranchId,
            original.Branch.NameAr,
            original.Branch.NameEn);

        var refund = new Invoice(client, branch, original.TotalPrice.Currency);
        refund.Type = InvoiceType.Refund;
        refund.OriginalInvoiceId = original.Id;
        refund.ExaminationId = original.ExaminationId;
        refund.AddHistory(InvoiceHistoryAction.RefundInvoiceCreated);

        return refund;
    }

    public static Invoice CreateAdjustment(Invoice original)
    {
        if (original.Type != InvoiceType.Invoice)
            throw new DomainException("Can only create adjustment for regular invoices.");

        var client = new ClientReference(
            original.Client.ClientId,
            original.Client.NameAr,
            original.Client.NameEn,
            original.Client.PhoneNumber,
            original.Client.Email);

        var branch = new BranchReference(
            original.Branch.BranchId,
            original.Branch.NameAr,
            original.Branch.NameEn);

        var adjustment = new Invoice(client, branch, original.TotalPrice.Currency);
        adjustment.Type = InvoiceType.Adjustment;
        adjustment.OriginalInvoiceId = original.Id;
        adjustment.ExaminationId = original.ExaminationId;
        adjustment.AddHistory(InvoiceHistoryAction.Created);

        return adjustment;
    }

    public void SetNotes(string? notes) => Notes = Normalize(notes);
    public void SetDueDate(DateTime? dueDate) => DueDate = dueDate;

    public void SetDiscount(Money discount)
    {
        if (discount.Amount < 0)
            throw new DomainException("Discount cannot be negative.");
        if (discount.Amount > TotalPrice.Amount)
            throw new DomainException("Discount cannot exceed subtotal.");

        DiscountAmount = discount;
        RecalculateTotal();
        AddHistory(InvoiceHistoryAction.DiscountSet, $"{discount.Amount} {discount.Currency}");
    }

    public void Update(string? notes, DateTime? dueDate)
    {
        EnsureEditable();
        Notes   = Normalize(notes);
        DueDate = dueDate;
        AddHistory(InvoiceHistoryAction.Updated);
    }

    public void UpdateClientSnapshot(ClientReference clientRef)
    {
        EnsureEditable();
        if (clientRef.ClientId == Guid.Empty) throw new DomainException("Client is required.");
        Client = clientRef;
    }

    public void UpdateBranchSnapshot(BranchReference branchRef)
    {
        EnsureEditable();
        if (branchRef.BranchId == Guid.Empty) throw new DomainException("Branch is required.");
        Branch = branchRef;
    }

    public void AddItem(string description, Money unitPrice,
        Guid? serviceId = null, string? serviceNameAr = null, string? serviceNameEn = null,
        decimal adjustmentAmount = 0)
    {
        EnsureItemsEditable();

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Item description is required.");

        _items.Add(new InvoiceItem(description, unitPrice, serviceId, serviceNameAr, serviceNameEn, adjustmentAmount));
        RecalculateTotal();
        AddHistory(InvoiceHistoryAction.ItemAdded, description);
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureItemsEditable();

        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return;

        _items.Remove(item);
        RecalculateTotal();
        AddHistory(InvoiceHistoryAction.ItemRemoved);
    }

    public void UpdateItem(Guid itemId, string description, Money unitPrice, decimal adjustmentAmount = 0)
    {
        EnsureItemsEditable();

        var item = _items.FirstOrDefault(x => x.Id == itemId)
                   ?? throw new DomainException("Item not found.");

        item.Update(description, unitPrice, adjustmentAmount);
        RecalculateTotal();
        AddHistory(InvoiceHistoryAction.ItemUpdated, description);
    }

    public void AddPayment(Money amount, Guid methodId, string methodNameAr, string methodNameEn, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add payment to a cancelled invoice.");

        _payments.Add(new InvoicePayment(amount, methodId, methodNameAr, methodNameEn, PaymentType.Payment, notes));
        UpdatePaymentStatus();
        AddHistory(InvoiceHistoryAction.PaymentAdded, $"{amount.Amount} {amount.Currency}");
    }

    public void AddRefund(Money amount, Guid methodId, string methodNameAr, string methodNameEn, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add refund to a cancelled invoice.");

        if (amount.Amount > TotalPaid - TotalRefunded)
            throw new DomainException("Refund amount exceeds paid amount.");

        _payments.Add(new InvoicePayment(amount, methodId, methodNameAr, methodNameEn, PaymentType.Refund, notes));
        UpdatePaymentStatus();
        AddHistory(InvoiceHistoryAction.RefundAdded, $"{amount.Amount} {amount.Currency}");
    }

    public void SetInvoiceNumber(string invoiceNumber)
    {
        if (!string.IsNullOrWhiteSpace(InvoiceNumber))
            throw new DomainException("Invoice number already assigned.");
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Invoice number is required.");

        InvoiceNumber = invoiceNumber;
    }

    public void EnsureDeletable()
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Paid invoices cannot be deleted.");
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cancelled invoices cannot be deleted.");
    }

    public void MarkAsRefunded()
    {
        if (Type != InvoiceType.Refund)
            throw new DomainException("Only refund invoices can be marked as refunded.");

        Status = InvoiceStatus.Refunded;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Paid invoice cannot be cancelled.");

        Status = InvoiceStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = Normalize(reason);
        AddHistory(InvoiceHistoryAction.Cancelled, reason);
    }

    /// <summary>
    /// Force-cancel regardless of current status. Used when the parent examination is cancelled.
    /// </summary>
    public void ForceCancel(string? reason = null)
    {
        if (Status == InvoiceStatus.Cancelled)
            return;

        Status = InvoiceStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = Normalize(reason);
        AddHistory(InvoiceHistoryAction.Cancelled, reason);
    }

    private void UpdatePaymentStatus()
    {
        if (Status == InvoiceStatus.Cancelled)
            return;

        var netPaid = TotalPaid - TotalRefunded;

        if (TotalRefunded > 0 && TotalRefunded >= TotalPaid)
            Status = InvoiceStatus.Refunded;
        else if (netPaid >= TotalWithTax.Amount)
            Status = InvoiceStatus.Paid;
        else
            Status = InvoiceStatus.Issued;
    }

    private void EnsureEditable()
    {
        if (Status != InvoiceStatus.Issued)
            throw new DomainException("Invoice can only be updated in Issued status.");
    }

    private void EnsureItemsEditable()
    {
        if (Status != InvoiceStatus.Issued)
            throw new DomainException("You can modify invoice items only in Issued status.");
    }

    private void RecalculateTotal()
    {
        if (_items.Count == 0)
        {
            TotalPrice   = Money.Zero(TotalPrice.Currency);
            TaxAmount    = Money.Zero(TotalPrice.Currency);
            TotalWithTax = Money.Zero(TotalPrice.Currency);
            return;
        }

        var total = Money.Zero(_items[0].TotalPrice.Currency);
        foreach (var i in _items)
            total = total.Add(i.TotalPrice);

        TotalPrice = total;

        // Clamp discount to subtotal (only for positive totals)
        if (total.Amount >= 0 && DiscountAmount.Amount > total.Amount)
            DiscountAmount = total;

        var afterDiscount = total.Amount - DiscountAmount.Amount;
        TaxAmount    = Money.CreateAllowNegative(Math.Round(afterDiscount * TaxRate, 2), total.Currency);
        TotalWithTax = Money.CreateAllowNegative(Math.Round(afterDiscount + TaxAmount.Amount, 2), total.Currency);
    }

    private void AddHistory(InvoiceHistoryAction action, string? details = null)
    {
        _history.Add(new InvoiceHistory(Id, action, details));
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
