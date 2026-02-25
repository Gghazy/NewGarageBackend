namespace Garage.Contracts.Invoices;

public sealed record UpdateInvoiceRequest(
    // -- Client ---------------------------------------------------------------
    Guid    ClientId,
    Guid    BranchId,

    // -- Meta -----------------------------------------------------------------
    string? Notes,
    DateTime? DueDate,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemRequest>? Items   // replaces all items (Draft only)
);
