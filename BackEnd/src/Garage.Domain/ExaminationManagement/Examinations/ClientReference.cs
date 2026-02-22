using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Domain.ExaminationManagement.Examinations
{
    public sealed record ClientReference(
     Guid ClientId,
     string NameAr,
     string NameEn,
     string PhoneNumber,
     string? Email
 );
}
