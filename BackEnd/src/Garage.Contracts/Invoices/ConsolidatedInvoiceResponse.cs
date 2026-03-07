namespace Garage.Contracts.Invoices;

public sealed record ConsolidatedInvoiceResponse(
    // -- Client ---------------------------------------------------------------
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,

    // -- Branch ---------------------------------------------------------------
    string? BranchNameAr,
    string? BranchNameEn,

    // -- Vehicle --------------------------------------------------------------
    string? ManufacturerNameAr,
    string? ManufacturerNameEn,
    string? CarMarkNameAr,
    string? CarMarkNameEn,
    int?    Year,
    string? Color,
    string? Vin,
    string? PlateLetters,
    string? PlateNumbers,
    decimal? Mileage,
    string? MileageUnit,

    // -- Items ----------------------------------------------------------------
    List<ConsolidatedItemResponse> Items,

    // -- Financials -----------------------------------------------------------
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TaxAmount,
    decimal TotalWithTax,
    decimal TotalPaid,
    decimal Balance,
    string  Currency,
    int     InvoiceCount
);

public sealed record ConsolidatedItemResponse(
    string  Description,
    string? ServiceNameAr,
    string? ServiceNameEn,
    decimal UnitPrice,
    decimal TotalPrice,
    string  Currency
);
