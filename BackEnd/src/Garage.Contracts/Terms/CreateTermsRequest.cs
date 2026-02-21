namespace Garage.Contracts.Terms
{
    public record CreateTermsRequest(
        string TermsAndConditionsAr,
        string TermsAndConditionsEn,
        string CancelWarrantyDocumentAr,
        string CancelWarrantyDocumentEn
    );
}
