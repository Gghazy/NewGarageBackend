using Garage.Application.Abstractions;
using Garage.Contracts.Common;
using Garage.Contracts.Services;
using Garage.Domain.Services.Entity;
using Garage.Domain.Services.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Services.Queries.GetAll;
public sealed class GetAllServiceHandler(IReadRepository<Service> repo)
    : IRequestHandler<GetAllServiceQuery, List<ServiceDto>>
{
    public async Task<List<ServiceDto>> Handle(GetAllServiceQuery request, CancellationToken ct)
    {
       return await repo.Query()
            .Select(x => new ServiceDto
            (
                x.Id,
                x.NameAr,
                x.NameEn,
                new List<ServiceStageDto>()
            )).ToListAsync(ct);
    }
}


