using Garage.Domain.Common.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Cranes.Entity
{
    public class Crane : LookupBase
    {
        public Crane() { }
        public Crane(string nameAr, string nameEn) : base(nameAr, nameEn) { }
    }
}
