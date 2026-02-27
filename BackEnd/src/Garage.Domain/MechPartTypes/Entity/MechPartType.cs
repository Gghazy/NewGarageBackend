using Garage.Domain.Common.Primitives;
using Garage.Domain.MechParts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.MechPartTypes.Entity
{
    public class MechPartType:LookupBase
    {
        private readonly List<MechPart> _mechParts = new();

        public MechPartType() { }
        public MechPartType(string nameAr, string nameEn) : base(nameAr, nameEn) { }

        public IReadOnlyCollection<MechPart> MechParts => _mechParts;

    }
}
