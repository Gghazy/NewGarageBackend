namespace Garage.Contracts.Examinations;

public sealed record ExaminationDto(
    Guid    Id,

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
    string? Notes,

    // ── Items ─────────────────────────────────────────────────────────────────
    List<ExaminationItemDto> Items,

    DateTime CreatedAtUtc
);

public sealed record ExaminationItemDto(
    Guid    Id,
    Guid    ServiceId,
    string  ServiceNameAr,
    string  ServiceNameEn,
    int     Quantity,
    decimal? OverridePrice,
    string  Status,
    string? Notes
);
