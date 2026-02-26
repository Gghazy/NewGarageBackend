namespace Garage.Contracts.Invoices;

public sealed record InvoiceDto(
    Guid    Id,
    string? InvoiceNumber,
    Guid?   ExaminationId,
    Guid?   OriginalInvoiceId,

    // -- Type & Status -----------------------------------------------------------
    string  Type,
    string  Status,

    // -- Client ---------------------------------------------------------------
    Guid    ClientId,
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,

    // -- Branch ---------------------------------------------------------------
    Guid    BranchId,
    string  BranchNameAr,
    string  BranchNameEn,

    // -- Meta -----------------------------------------------------------------
    string? Notes,
    DateTime? DueDate,

    // -- Financials -----------------------------------------------------------
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TaxRate,
    decimal TaxAmount,
    decimal TotalWithTax,
    string  Currency,
    decimal TotalPaid,
    decimal TotalRefunded,
    decimal Balance,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemDto> Items,

    // -- Payments -------------------------------------------------------------
    List<InvoicePaymentDto> Payments,

    // -- Related invoices (Refund / Adjustment linked to this one) ----------
    List<RelatedInvoiceDto> RelatedInvoices,

    DateTime CreatedAtUtc
);

public sealed record InvoiceItemDto(
    Guid    Id,
    string  Description,
    int     Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string  Currency,
    Guid?   ServiceId,
    string? ServiceNameAr,
    string? ServiceNameEn
);

public sealed record InvoicePaymentDto(
    Guid    Id,
    decimal Amount,
    string  Currency,
    string  Method,
    string  Type,
    string? Notes,
    DateTime CreatedAtUtc
);

public sealed record RelatedInvoiceDto(
    Guid    Id,
    string? InvoiceNumber,
    string  Type,
    string  Status,
    decimal TotalWithTax,
    string  Currency,
    DateTime CreatedAtUtc
);
