using Garage.Application.Abstractions;
using Garage.Contracts.CarMarks;
using Garage.Domain.CarMarkes.Entity;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.CarMarks.Queries.GetAll;

public sealed class GetAllCarMarksHandler(IReadRepository<CarMark> repo)
    : IRequestHandler<GetAllCarMarksQuery, List<CarMarkDto>>
{
    public async Task<List<CarMarkDto>> Handle(GetAllCarMarksQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Include(cm => cm.Manufacturer)
            .OrderByDescending(cm => cm.CreatedAtUtc)
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
