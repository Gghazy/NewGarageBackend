using Garage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.InteriorIssues.Entity
{
    public class InteriorIssue: LookupBase
    {
        private InteriorIssue() { }
        public InteriorIssue(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}

