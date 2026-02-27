namespace Garage.Contracts.Invoices;

public sealed record AddInvoicePaymentRequest(
    decimal Amount,
    Guid    MethodId,
    string? Currency,
    string? Notes
);
