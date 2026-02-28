using Garage.Domain.Common.Primitives;

namespace Garage.Domain.InvoiceManagement.Invoices;

public sealed class InvoiceHistory : Entity
{
    public Guid InvoiceId { get; private set; }
    public InvoiceHistoryAction Action { get; private set; }
    public string? Details { get; private set; }

    private InvoiceHistory() { }

    internal InvoiceHistory(
        Guid invoiceId,
        InvoiceHistoryAction action,
        string? details = null)
    {
        InvoiceId = invoiceId;
        Action = action;
        Details = details;
    }
}
