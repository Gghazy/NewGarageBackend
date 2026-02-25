namespace Garage.Contracts.Invoices;

public sealed record InvoiceDto(
    Guid    Id,
    string? InvoiceNumber,
    Guid?   ExaminationId,

    // -- Status ---------------------------------------------------------------
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
