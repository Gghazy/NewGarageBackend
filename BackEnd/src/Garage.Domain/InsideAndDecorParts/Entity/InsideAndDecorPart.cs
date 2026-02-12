using Garage.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.InsideAndDecorParts.Entity
{
    public class InsideAndDecorPart:LookupBase
    {
        public InsideAndDecorPart() { }
        public InsideAndDecorPart(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}

