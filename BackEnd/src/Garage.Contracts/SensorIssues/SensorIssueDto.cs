using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.SensorIssues
{

    public record SensorIssueDto(Guid Id, string NameAr, string NameEn, string code);

}

