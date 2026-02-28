namespace Garage.Contracts.Examinations;

public sealed record CreateExaminationRequest(
    // ── Client ───────────────────────────────────────────────────────────────
    // null  → create new client
    // value → find existing and update data
    Guid?   ClientId,
    string  ClientType,       // "Individual" | "Company"
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,
    string? ClientEmail,
    Guid?   ClientResourceId,

    // Individual client address (single free-text field)
    string? IndividualAddress,

    // Company-specific fields
    string? CommercialRegister,
    string? TaxNumber,

    // Company structured address
    string? StreetName,
    string? AdditionalStreetName,
    string? CityName,
    string? PostalZone,
    string? CountrySubentity,
    string? CountryCode,
    string? BuildingNumber,
    string? CitySubdivisionName,

    // ── Branch ───────────────────────────────────────────────────────────────
    Guid   BranchId,

    // ── Examination meta ─────────────────────────────────────────────────────
    string  Type,           // "Regular" | "Warranty" | "PrePurchase"
    bool    HasWarranty,
    string? Notes,

    // ── Vehicle ───────────────────────────────────────────────────────────────
    Guid    ManufacturerId,
    Guid    CarMarkId,
    int?    Year,
    string? Color,
    string? Vin,
    bool    HasPlate,
    string? PlateLetters,   // أحرف اللوحة
    string? PlateNumbers,   // أرقام اللوحة
    decimal? Mileage,
    string   MileageUnit,   // "Km" | "Mile"
    string?  Transmission,  // "Automatic" | "Manual" | "CVT" | "SemiAutomatic"

    // ── Services ──────────────────────────────────────────────────────────────
    List<ExaminationItemRequest> Items,

    // ── Workflow ──────────────────────────────────────────────────────────────
    bool StartAfterSave = false
) : IExaminationRequest
{
    List<ExaminationItemRequest>? IExaminationRequest.Items => Items;
}

public sealed record ExaminationItemRequest(
    Guid     ServiceId,
    int      Quantity = 1,
    decimal? OverridePrice = null
);
