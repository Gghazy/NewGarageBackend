using Garage.Domain.Common.Primitives;
using Garage.Domain.MechPartTypes.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.MechParts.Entities
{


    public class MechPart : AggregateRoot
    {
        public string NameAr { get; private set; }
        public string NameEn { get; private set; }
        public Guid MechPartTypeId { get; private set; }

        public virtual MechPartType MechPartType { get; private set; }
        private MechPart() { }
        public MechPart(string nameAr, string nameEn, Guid mechPartTypeId)
        {
            MechPartTypeId = mechPartTypeId;
            NameAr = nameAr;
            NameEn = nameEn;
        }
        public void Update(string nameAr, string nameEn, Guid mechPartTypeId)
        {
            MechPartTypeId = mechPartTypeId;
            NameAr = nameAr;
            NameEn = nameEn;
        }



    }
}
