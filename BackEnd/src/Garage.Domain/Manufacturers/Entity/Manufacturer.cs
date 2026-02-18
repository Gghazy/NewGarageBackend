using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Manufacturers.Entity
{
    public class Manufacturer:LookupBase
    {
        public Manufacturer() { }
        public Manufacturer(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
