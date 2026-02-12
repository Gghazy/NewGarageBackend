using Garage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.ExteriorBodyIssues.Entity
{
    public class ExteriorBodyIssue :LookupBase
    {
        public ExteriorBodyIssue() { }
        public ExteriorBodyIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}

