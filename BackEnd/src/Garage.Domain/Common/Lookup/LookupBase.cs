using Garage.Domain.Common.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.Common.Lookup
{
    public class LookupBase:AggregateRoot
    {
        public string NameAr { get; protected set; } = null!;
        public string NameEn { get; protected set; } = null!;

        protected LookupBase() { }

        protected LookupBase(string ar, string en)
            => Update(ar, en);

        public void Update(string ar, string en)
        {
            NameAr = ar;
            NameEn = en;
        }
    }
}

