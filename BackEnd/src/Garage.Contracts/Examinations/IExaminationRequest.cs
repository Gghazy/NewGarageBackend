namespace Garage.Contracts.Examinations;

public interface IExaminationRequest
{
    // ── Client ──────────────────────────────────────────────────────
    string  ClientType           { get; }
    string  ClientNameAr         { get; }
    string  ClientNameEn         { get; }
    string  ClientPhone          { get; }
    string? ClientEmail          { get; }
    Guid?   ClientResourceId     { get; }
    string? IndividualAddress    { get; }
    string? CommercialRegister   { get; }
    string? TaxNumber            { get; }
    string? StreetName           { get; }
    string? AdditionalStreetName { get; }
    string? CityName             { get; }
    string? PostalZone           { get; }
    string? CountrySubentity     { get; }
    string? CountryCode          { get; }
    string? BuildingNumber       { get; }
    string? CitySubdivisionName  { get; }

    // ── Vehicle ─────────────────────────────────────────────────────
    Guid     ManufacturerId { get; }
    Guid     CarMarkId      { get; }
    int?     Year           { get; }
    string?  Color          { get; }
    string?  Vin            { get; }
    bool     HasPlate       { get; }
    string?  PlateLetters   { get; }
    string?  PlateNumbers   { get; }
    decimal? Mileage        { get; }
    string   MileageUnit    { get; }
    string?  Transmission   { get; }

    // ── Meta ────────────────────────────────────────────────────────
    bool    HasWarranty  { get; }
    string? MarketerCode { get; }
    string? Notes        { get; }

    // ── Items ───────────────────────────────────────────────────────
    List<ExaminationItemRequest>? Items { get; }
}
