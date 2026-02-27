using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.MechParts
{

    public record MechPartResponse(
        Guid Id,
        string NameAr,
        string NameEn,
        string MechPartTypeNameAr,
        string MechPartTypeNameEn,
        Guid MechPartTypeId);
}
