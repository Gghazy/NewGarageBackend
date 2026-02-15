using Garage.Application.Abstractions;
using Garage.Contracts.Services;
using Garage.Domain.Services.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


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


