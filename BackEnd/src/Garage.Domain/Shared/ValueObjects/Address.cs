using Garage.Domain.Common.Exceptions;
using Garage.Domain.Common.Primitives;


namespace Garage.Domain.Shared.ValueObjects;

public sealed class Address : ValueObject
{
    public string StreetName { get; }
    public string AdditionalStreetName { get; }
    public string CityName { get; }
    public string PostalZone { get; }
    public string CountrySubentity { get; }
    public string CountryCode { get; }
    public string BuildingNumber { get; }
    public string CitySubdivisionName { get; }

    private Address() { } 

    public Address(
        string streetName,
        string? additionalStreetName,
        string cityName,
        string postalZone,
        string? countrySubentity,
        string countryCode,
        string buildingNumber,
        string? citySubdivisionName)
    {
        if (string.IsNullOrWhiteSpace(streetName))
            throw new DomainException("Street is required");

        if (string.IsNullOrWhiteSpace(cityName))
            throw new DomainException("City is required");

        if (string.IsNullOrWhiteSpace(countryCode))
            throw new DomainException("Country code is required");

        StreetName = streetName.Trim();
        AdditionalStreetName = additionalStreetName?.Trim() ?? string.Empty;
        CityName = cityName.Trim();
        PostalZone = postalZone.Trim();
        CountrySubentity = countrySubentity?.Trim() ?? string.Empty;
        CountryCode = countryCode.Trim();
        BuildingNumber = buildingNumber.Trim();
        CitySubdivisionName = citySubdivisionName?.Trim() ?? string.Empty;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return StreetName;
        yield return AdditionalStreetName;
        yield return CityName;
        yield return PostalZone;
        yield return CountrySubentity;
        yield return CountryCode;
        yield return BuildingNumber;
        yield return CitySubdivisionName;
    }
}
