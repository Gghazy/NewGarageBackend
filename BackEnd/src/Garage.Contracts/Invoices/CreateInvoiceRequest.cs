namespace Garage.Contracts.Invoices;

public sealed record CreateInvoiceRequest(
    // -- Client ---------------------------------------------------------------
    Guid    ClientId,
    Guid    BranchId,

    // -- Meta -----------------------------------------------------------------
    string? Notes,
    DateTime? DueDate,

    // -- Discount -------------------------------------------------------------
    decimal? Discount,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemRequest> Items
);

public sealed record InvoiceItemRequest(
    string  Description,
    decimal UnitPrice,
    Guid?   ServiceId
);
