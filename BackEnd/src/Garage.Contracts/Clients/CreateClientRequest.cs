

namespace Garage.Contracts.Clients
{
    public sealed record CreateClientRequest(
    // Identity
    string Email,

    // Common
    string Type,
    string NameAr,
    string NameEn,
    string PhoneNumber,

    // Company
    string? CommercialRegister,
    string? TaxNumber,
    Guid? ResourceId,


    // Address

    string? Address,
    string? StreetName,
    string? AdditionalStreetName,
    string? CityName,
    string? PostalZone,
    string? CountrySubentity,
    string? CountryCode,
    string? BuildingNumber,
    string? CitySubdivisionName
);
}
