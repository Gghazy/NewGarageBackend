using Garage.Application.Abstractions;
using Garage.Contracts.CarMarks;
using Garage.Domain.CarMarkes.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.CarMarks.Queries.GetByManufacturer;

public sealed class GetCarMarksByManufacturerHandler(IReadRepository<CarMark> repo)
    : IRequestHandler<GetCarMarksByManufacturerQuery, List<CarMarkDto>>
{
    public async Task<List<CarMarkDto>> Handle(GetCarMarksByManufacturerQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Include(cm => cm.Manufacturer)
            .Where(cm => cm.ManufacturerId == request.ManufacturerId)
            .OrderBy(cm => cm.NameAr)
            .Select(cm => new CarMarkDto(
                cm.Id,
                cm.NameAr,
                cm.NameEn,
                cm.ManufacturerId,
                cm.Manufacturer.NameAr,
                cm.Manufacturer.NameEn))
            .ToListAsync(ct);
    }
}
