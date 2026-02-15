using Garage.Domain.Common.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.CarMarkes.Entity
{
    public class CarMark: LookupBase
    {
        public CarMark() { }
        public CarMark(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
