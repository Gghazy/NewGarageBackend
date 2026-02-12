using Cashif.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Domain.RoadTestIssues.Entity
{
    public class RoadTestIssue :LookupBase
    {
        public RoadTestIssue() { }
        public RoadTestIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
