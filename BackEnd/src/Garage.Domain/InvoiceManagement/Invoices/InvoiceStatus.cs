namespace Garage.Domain.InvoiceManagement.Invoices;

public enum InvoiceStatus
{
    Draft = 1,
    Issued = 2,
    Paid = 3,
    Cancelled = 5,
    Refunded = 7
}
