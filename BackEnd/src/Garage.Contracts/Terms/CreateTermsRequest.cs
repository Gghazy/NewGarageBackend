using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Terms
{

    public record CreateTermsRequest(
        string TermsAndCondtionsAr,
        string TermsAndCondtionsEn,
        string CancelWarrantyDocumentAr,
        string CancelWarrantyDocumentEn
    );

}
