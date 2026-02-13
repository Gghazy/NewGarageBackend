using Garage.Domain.Common.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.InteriorBodyIssues.Entity
{
    public class InteriorBodyIssue: LookupBase
    {
        private InteriorBodyIssue() { }
        public InteriorBodyIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}

