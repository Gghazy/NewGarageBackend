using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Terms.Entity
{
    public class Term:AggregateRoot
    {
        public string TermsAndCondtionsAr { get; set; }
        public string TermsAndCondtionsEn { get; set; }
        public string CancelWarrantyDocumentAr { get; set; }
        public string CancelWarrantyDocumentEn { get; set; }

        public Term() { }
        public Term(string termsAndCondtionsAr, string termsAndCondtionsEn, string cancelWarrantyDocumentAr, string cancelWarrantyDocumentEn)
        {
            TermsAndCondtionsAr = termsAndCondtionsAr;
            TermsAndCondtionsEn = termsAndCondtionsEn;
            CancelWarrantyDocumentAr = cancelWarrantyDocumentAr;
            CancelWarrantyDocumentEn = cancelWarrantyDocumentEn;
        }

        public void Update(string termsAndCondtionsAr, string termsAndCondtionsEn, string cancelWarrantyDocumentAr, string cancelWarrantyDocumentEn)
        {
            TermsAndCondtionsAr = termsAndCondtionsAr;
            TermsAndCondtionsEn = termsAndCondtionsEn;
            CancelWarrantyDocumentAr = cancelWarrantyDocumentAr;
            CancelWarrantyDocumentEn = cancelWarrantyDocumentEn;
        }


    }
}
