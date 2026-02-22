

namespace Garage.Contracts.Clients;

public record ClientDto(
    Guid Id,
    string TypeAr,
    string TypeEn,
    string NameAr,
    string NameEn,
    string PhoneNumber,
    string? TaxNumber,
    string? CommercialRegister,
    string Email,
    string? SourceNameEn,
    string? SourceNameAr,
    Guid? SourceId,
    // Individual address
    string? Address,
    // Company address
    string? StreetName,
    string? AdditionalStreetName,
    string? CityName,
    string? PostalZone,
    string? CountrySubentity,
    string? CountryCode,
    string? BuildingNumber,
    string? CitySubdivisionName
   );

