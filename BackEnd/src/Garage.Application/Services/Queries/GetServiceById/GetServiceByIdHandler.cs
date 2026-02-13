using Garage.Application.Abstractions;
using Garage.Contracts.Services;
using Garage.Domain.Services.Entities;
using Garage.Domain.Services.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Services.Queries.GetServiceById;

public sealed class GetServiceByIdHandler(IReadRepository<Service> repo)
 : IRequestHandler<GetServiceByIdQuery, ServiceDto>
{
    public async Task<ServiceDto> Handle(GetServiceByIdQuery request, CancellationToken ct)
    {
        var service = 
            await repo.Query()
                      .Include(x => x.Stages)
                      .FirstOrDefaultAsync(x => x.Id == request.Id, ct)
                     ?? throw new KeyNotFoundException("Service not found");

        var stages = service.Stages
            .Select(x =>
            {
                var s = Stage.FromValue(x.StageValue);
                return new ServiceStageDto(
                    x.StageValue,
                    s.Name,
                    s.NameAr
                );
            }).ToList();

        return new ServiceDto(service.Id, service.NameAr, service.NameEn, stages);
    }
}
