namespace Garage.Contracts.Examinations;

public sealed record UpdateExaminationRequest(
    // ── Client ───────────────────────────────────────────────────────────────
    string  ClientType,       // "Individual" | "Company" | "Government"
    string  ClientNameAr,
    string  ClientNameEn,
    string  ClientPhone,
    string? ClientEmail,
    Guid?   ClientResourceId,

    // Individual address
    string? IndividualAddress,

    // Company-specific
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

    // ── Examination meta ──────────────────────────────────────────────────────
    bool    HasWarranty,
    bool    HasPhotos,
    string? MarketerCode,
    string? Notes,

    // ── Vehicle ───────────────────────────────────────────────────────────────
    Guid    ManufacturerId,
    Guid    CarMarkId,
    int?    Year,
    string? Color,
    string? Vin,
    bool    HasPlate,
    string? PlateLetters,
    string? PlateNumbers,
    decimal? Mileage,
    string   MileageUnit,    // "Km" | "Mile"
    string?  Transmission,   // "Automatic" | "Manual" | "CVT" | "SemiAutomatic"

    // ── Services ──────────────────────────────────────────────────────────────
    List<ExaminationItemRequest>? Items   // replaces all items (Draft only)
);
