namespace Garage.Contracts.Invoices;

public sealed record UpdateInvoiceRequest(
    // -- Client ---------------------------------------------------------------
    Guid    ClientId,
    Guid    BranchId,

    // -- Meta -----------------------------------------------------------------
    string? Notes,
    DateTime? DueDate,

    // -- Discount -------------------------------------------------------------
    decimal? Discount,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemRequest>? Items   // replaces all items (Issued only)
);
