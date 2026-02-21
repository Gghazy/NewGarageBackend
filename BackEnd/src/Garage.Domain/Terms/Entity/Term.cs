using Garage.Domain.Common.Primitives;

namespace Garage.Domain.Terms.Entity
{
    public class Term:AggregateRoot
    {
        public string TermsAndConditionsAr { get; set; }
        public string TermsAndConditionsEn { get; set; }
        public string CancelWarrantyDocumentAr { get; set; }
        public string CancelWarrantyDocumentEn { get; set; }

        public Term() { }
        public Term(string termsAndConditionsAr, string termsAndConditionsEn, string cancelWarrantyDocumentAr, string cancelWarrantyDocumentEn)
        {
            TermsAndConditionsAr = termsAndConditionsAr;
            TermsAndConditionsEn = termsAndConditionsEn;
            CancelWarrantyDocumentAr = cancelWarrantyDocumentAr;
            CancelWarrantyDocumentEn = cancelWarrantyDocumentEn;
        }

        public void Update(string termsAndConditionsAr, string termsAndConditionsEn, string cancelWarrantyDocumentAr, string cancelWarrantyDocumentEn)
        {
            TermsAndConditionsAr = termsAndConditionsAr;
            TermsAndConditionsEn = termsAndConditionsEn;
            CancelWarrantyDocumentAr = cancelWarrantyDocumentAr;
            CancelWarrantyDocumentEn = cancelWarrantyDocumentEn;
        }


    }
}
