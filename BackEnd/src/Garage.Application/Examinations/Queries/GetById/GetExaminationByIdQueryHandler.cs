using Domain.ExaminationManagement.Examinations;
using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetById;

public sealed class GetExaminationByIdQueryHandler(IReadRepository<Examination> repo)
    : BaseQueryHandler<GetExaminationByIdQuery, ExaminationDto?>
{
    public override async Task<ExaminationDto?> Handle(GetExaminationByIdQuery request, CancellationToken ct)
    {
        return await repo.Query()
            .Where(e => e.Id == request.Id)
            .Select(ExaminationProjection.ToDto)
            .FirstOrDefaultAsync(ct);
    }
}
