namespace Garage.Domain.InvoiceManagement.Invoices;

public enum InvoiceHistoryAction
{
    Created = 1,
    Updated = 2,
    Cancelled = 3,
    Deleted = 4,
    PaymentAdded = 5,
    RefundAdded = 6,
    DiscountSet = 7,
    ItemAdded = 8,
    ItemRemoved = 9,
    ItemUpdated = 10,
    StatusChanged = 11,
    RefundInvoiceCreated = 12,
}
