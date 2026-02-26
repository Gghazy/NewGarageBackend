namespace Garage.Contracts.Invoices;

public sealed record AddInvoicePaymentRequest(
    decimal Amount,
    string  Method,   // "Cash" | "Card" | "BankTransfer" | "Cheque"
    string? Currency, // defaults to "SAR" if null
    string? Notes
);
