namespace Garage.Contracts.Examinations;

public sealed record ExaminationDto(
    Guid    Id,
    string? InvoiceNumber,

    // ── Status ────────────────────────────────────────────────────────────────
    string  Status,
    string  Type,

    // ── Client ────────────────────────────────────────────────────────────────
    Guid    ClientId,
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,

    // ── Branch ────────────────────────────────────────────────────────────────
    Guid    BranchId,
    string  BranchNameAr,
    string  BranchNameEn,

    // ── Vehicle ───────────────────────────────────────────────────────────────
    Guid    VehicleId,
    Guid    ManufacturerId,
    string  ManufacturerNameAr,
    string  ManufacturerNameEn,
    Guid    CarMarkId,
    string  CarMarkNameAr,
    string  CarMarkNameEn,
    int?    Year,
    string? Color,
    string? Vin,
    bool    HasPlate,
    string? PlateLetters,
    string? PlateNumbers,
    decimal? Mileage,
    string   MileageUnit,
    string?  Transmission,

    // ── Examination meta ──────────────────────────────────────────────────────
    bool    HasWarranty,
    bool    HasPhotos,
    string? MarketerCode,
    string? Notes,

    // ── Financials ────────────────────────────────────────────────────────────
    decimal SubTotal,
    decimal TaxRate,
    decimal TaxAmount,
    decimal TotalWithTax,
    string  Currency,
    decimal TotalPaid,
    decimal TotalRefunded,
    decimal Balance,

    // ── Items ─────────────────────────────────────────────────────────────────
    List<ExaminationItemDto> Items,

    // ── Payments ──────────────────────────────────────────────────────────────
    List<PaymentDto> Payments,

    DateTime CreatedAtUtc
);

public sealed record ExaminationItemDto(
    Guid    Id,
    Guid    ServiceId,
    string  ServiceNameAr,
    string  ServiceNameEn,
    decimal Price,
    string  Currency,
    string  Status,
    string? Notes
);

public sealed record PaymentDto(
    Guid    Id,
    decimal Amount,
    string  Currency,
    string  Method,
    string  Type,
    string? Notes,
    DateTime CreatedAtUtc
);
