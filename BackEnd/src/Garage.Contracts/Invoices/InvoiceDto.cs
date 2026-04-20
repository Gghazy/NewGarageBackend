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
    decimal NetTotal,
    string  Currency,

    // -- Items ----------------------------------------------------------------
    List<InvoiceItemDto> Items,

    // -- Payments -------------------------------------------------------------
    List<InvoicePaymentDto> Payments,

    DateTime CreatedAtUtc
);

public sealed record InvoicePaymentDto(
    Guid    Id,
    decimal Amount,
    string  Currency,
    Guid    MethodId,
    string  MethodNameAr,
    string  MethodNameEn,
    string  Type,
    string? Notes,
    DateTime CreatedAtUtc
);

public sealed record InvoiceItemDto(
    Guid    Id,
    string  Description,
    decimal UnitPrice,
    decimal TotalPrice,
    string  Currency,
    Guid?   ServiceId,
    string? ServiceNameAr,
    string? ServiceNameEn,
    decimal AdjustmentAmount,
    decimal DiscountPercent,
    decimal DiscountAmount
);
