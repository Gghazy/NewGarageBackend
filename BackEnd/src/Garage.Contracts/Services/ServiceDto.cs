using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Contracts.Services
{
    public sealed record ServiceDto(
     Guid Id,
     string NameAr,
     string NameEn,
     IList<ServiceStageDto> Stages
    );


    public sealed record ServiceStageDto(
    int Id,
    string StageName,
    string StageNameAr
    );
}
