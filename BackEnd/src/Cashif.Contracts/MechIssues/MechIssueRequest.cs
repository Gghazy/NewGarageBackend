using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Contracts.MechIssues
{

    public record MechIssueRequest(string NameAr, string NameEn,Guid MechIssueTypeId);
}
