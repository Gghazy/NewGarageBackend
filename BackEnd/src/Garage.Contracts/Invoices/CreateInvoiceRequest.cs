namespace Garage.Contracts.Invoices;

public sealed record CreateInvoiceRequest(
    // -- Client ---------------------------------------------------------------
    Guid    ClientId,
    Guid    BranchId,

    // -- Meta -----------------------------------------------------------------
    string? Notes,
    DateTime? DueDate,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemRequest> Items
);

public sealed record InvoiceItemRequest(
    string  Description,
    int     Quantity,
    decimal UnitPrice,
    Guid?   ServiceId
);
