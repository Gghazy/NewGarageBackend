namespace Garage.Contracts.Terms
{
    public record TermsDto
    (
        Guid? Id,
        string TermsAndConditionsAr,
        string TermsAndConditionsEn,
        string CancelWarrantyDocumentAr,
        string CancelWarrantyDocumentEn
    );
}
