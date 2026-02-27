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
        refund.AddItem("Refund", 1, refundAmount);

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
    }

    public void Update(string? notes, DateTime? dueDate)
    {
        EnsureEditable();
        Notes   = Normalize(notes);
        DueDate = dueDate;
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

    public void AddItem(string description, int quantity, Money unitPrice,
        Guid? serviceId = null, string? serviceNameAr = null, string? serviceNameEn = null)
    {
        EnsureItemsEditable();

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Item description is required.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        _items.Add(new InvoiceItem(description, quantity, unitPrice, serviceId, serviceNameAr, serviceNameEn));
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureItemsEditable();

        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return;

        _items.Remove(item);
        RecalculateTotal();
    }

    public void UpdateItem(Guid itemId, string description, int quantity, Money unitPrice)
    {
        EnsureItemsEditable();

        var item = _items.FirstOrDefault(x => x.Id == itemId)
                   ?? throw new DomainException("Item not found.");

        item.Update(description, quantity, unitPrice);
        RecalculateTotal();
    }

    public void AddPayment(Money amount, Guid methodId, string methodNameAr, string methodNameEn, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add payment to a cancelled invoice.");

        _payments.Add(new InvoicePayment(amount, methodId, methodNameAr, methodNameEn, PaymentType.Payment, notes));
        UpdatePaymentStatus();
    }

    public void AddRefund(Money amount, Guid methodId, string methodNameAr, string methodNameEn, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add refund to a cancelled invoice.");

        var totalPaid = _payments
            .Where(p => p.Type == PaymentType.Payment)
            .Sum(p => p.Amount.Amount);
        var totalRefunded = _payments
            .Where(p => p.Type == PaymentType.Refund)
            .Sum(p => p.Amount.Amount);

        if (amount.Amount > totalPaid - totalRefunded)
            throw new DomainException("Refund amount exceeds paid amount.");

        _payments.Add(new InvoicePayment(amount, methodId, methodNameAr, methodNameEn, PaymentType.Refund, notes));
        UpdatePaymentStatus();
    }

    public void SetInvoiceNumber(string invoiceNumber)
    {
        if (!string.IsNullOrWhiteSpace(InvoiceNumber))
            throw new DomainException("Invoice number already assigned.");
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Invoice number is required.");

        InvoiceNumber = invoiceNumber;
    }

    public void Cancel(string? reason = null)
    {
        if (Status == InvoiceStatus.Paid)
            throw new DomainException("Paid invoice cannot be cancelled.");

        Status = InvoiceStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
            Notes = Normalize(reason);
    }

    private void UpdatePaymentStatus()
    {
        if (Status == InvoiceStatus.Cancelled)
            return;

        var totalPaid = _payments
            .Where(p => p.Type == PaymentType.Payment)
            .Sum(p => p.Amount.Amount);
        var totalRefunded = _payments
            .Where(p => p.Type == PaymentType.Refund)
            .Sum(p => p.Amount.Amount);
        var netPaid = totalPaid - totalRefunded;

        if (totalRefunded > 0 && totalRefunded >= totalPaid)
            Status = InvoiceStatus.Refunded;
        else if (totalRefunded > 0)
            Status = InvoiceStatus.PartiallyRefunded;
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

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
