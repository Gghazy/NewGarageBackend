using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Shared;
using Domain.ExaminationManagement.Examinations;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class Invoice : AggregateRoot
{
    public ClientReference Client { get; private set; } = default!;
    public BranchReference Branch { get; private set; } = default!;

    public InvoiceStatus Status { get; private set; }
    public string? InvoiceNumber { get; private set; }
    public Guid? ExaminationId { get; private set; }
    public string? Notes { get; private set; }
    public DateTime? DueDate { get; private set; }

    public Money TotalPrice   { get; private set; } = Money.Zero("EGP");
    public decimal TaxRate    { get; private set; } = 0.15m;
    public Money TaxAmount    { get; private set; } = Money.Zero("EGP");
    public Money TotalWithTax { get; private set; } = Money.Zero("EGP");

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

        Status       = InvoiceStatus.Draft;
        TotalPrice   = Money.Zero(currency);
        TaxAmount    = Money.Zero(currency);
        TotalWithTax = Money.Zero(currency);
    }

    public static Invoice Create(
        ClientReference client,
        BranchReference branch,
        string currency = "EGP",
        Guid? examinationId = null)
    {
        if (client.ClientId == Guid.Empty)  throw new DomainException("Client is required.");
        if (branch.BranchId == Guid.Empty)  throw new DomainException("Branch is required.");
        if (string.IsNullOrWhiteSpace(currency)) throw new DomainException("Currency is required.");

        var invoice = new Invoice(client, branch, currency);
        invoice.ExaminationId = examinationId;
        return invoice;
    }

    public void SetNotes(string? notes) => Notes = Normalize(notes);
    public void SetDueDate(DateTime? dueDate) => DueDate = dueDate;

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
        EnsureDraft();

        if (string.IsNullOrWhiteSpace(description))
            throw new DomainException("Item description is required.");
        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero.");

        _items.Add(new InvoiceItem(description, quantity, unitPrice, serviceId, serviceNameAr, serviceNameEn));
        RecalculateTotal();
    }

    public void RemoveItem(Guid itemId)
    {
        EnsureDraft();

        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item is null) return;

        _items.Remove(item);
        RecalculateTotal();
    }

    public void UpdateItem(Guid itemId, string description, int quantity, Money unitPrice)
    {
        EnsureDraft();

        var item = _items.FirstOrDefault(x => x.Id == itemId)
                   ?? throw new DomainException("Item not found.");

        item.Update(description, quantity, unitPrice);
        RecalculateTotal();
    }

    public void AddPayment(Money amount, PaymentMethod method, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add payment to a cancelled invoice.");

        _payments.Add(new InvoicePayment(amount, method, PaymentType.Payment, notes));
        UpdatePaymentStatus();
    }

    public void AddRefund(Money amount, PaymentMethod method, string? notes)
    {
        if (Status == InvoiceStatus.Cancelled)
            throw new DomainException("Cannot add refund to a cancelled invoice.");

        var totalPaid     = _payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount);
        var totalRefunded = _payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount);
        var refundable    = totalPaid - totalRefunded;

        if (amount.Amount > refundable)
            throw new DomainException($"Refund amount ({amount.Amount}) exceeds refundable balance ({refundable}).");

        _payments.Add(new InvoicePayment(amount, method, PaymentType.Refund, notes));
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

    public void Issue()
    {
        EnsureDraft();
        if (_items.Count == 0)
            throw new DomainException("Cannot issue invoice without items.");

        Status = InvoiceStatus.Issued;
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
        if (Status == InvoiceStatus.Draft || Status == InvoiceStatus.Cancelled)
            return;

        var totalPaid     = _payments.Where(p => p.Type == PaymentType.Payment).Sum(p => p.Amount.Amount);
        var totalRefunded = _payments.Where(p => p.Type == PaymentType.Refund).Sum(p => p.Amount.Amount);
        var netPaid       = totalPaid - totalRefunded;

        if (netPaid >= TotalWithTax.Amount)
            Status = InvoiceStatus.Paid;
        else if (netPaid > 0)
            Status = InvoiceStatus.PartiallyPaid;
        else
            Status = InvoiceStatus.Issued;
    }

    private void EnsureEditable()
    {
        if (Status != InvoiceStatus.Draft && Status != InvoiceStatus.Issued && Status != InvoiceStatus.PartiallyPaid)
            throw new DomainException("Invoice can only be updated in Draft, Issued, or PartiallyPaid status.");
    }

    private void EnsureDraft()
    {
        if (Status != InvoiceStatus.Draft)
            throw new DomainException("You can modify invoice items only in Draft status.");
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

        TotalPrice   = total;
        TaxAmount    = Money.Create(Math.Round(total.Amount * TaxRate, 2), total.Currency);
        TotalWithTax = total.Add(TaxAmount);
    }

    private static string? Normalize(string? v)
        => string.IsNullOrWhiteSpace(v) ? null : v.Trim();
}
