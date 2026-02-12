using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Contracts.MechIssues
{

    public record MechIssueResponse(
        Guid Id,
        string NameAr, 
        string NameEn,
        string MechIssueTypeNameAr,
        string MechIssueTypeNameEn,
        Guid MechIssueTypeId);
}
