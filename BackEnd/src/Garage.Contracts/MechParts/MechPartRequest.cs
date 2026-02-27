using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.MechParts
{

    public record MechPartRequest(string NameAr, string NameEn,Guid MechPartTypeId);
}
