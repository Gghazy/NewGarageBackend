

namespace Garage.Contracts.Clients;

public record ClientDto(
    string TypeAr,
    string TypeEn,
    string NameAr,
    string NameEn,
    string PhoneNumber,
    string? TaxNumber,
    string? CommercialRegister,
    string Email,
    string SourceNameEn,
    string SourceNameAr
   );

