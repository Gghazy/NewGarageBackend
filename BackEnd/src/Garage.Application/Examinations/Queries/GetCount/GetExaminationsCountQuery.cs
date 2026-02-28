using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Examinations;
using Garage.Domain.ExaminationManagement.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetCount;

public sealed record GetExaminationsCountQuery(DateTime? From, DateTime? To, Guid? BranchId)
    : IRequest<ExaminationCountsByStatusDto>;

public sealed class GetExaminationsCountHandler(
    IApplicationDbContext db,
    IBranchAccessService branchAccess)
    : IRequestHandler<GetExaminationsCountQuery, ExaminationCountsByStatusDto>
{
    public async Task<ExaminationCountsByStatusDto> Handle(GetExaminationsCountQuery request, CancellationToken ct)
    {
        var accessibleBranches = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = db.Examinations.Where(e => !e.IsDeleted);

        if (request.From.HasValue)
            query = query.Where(e => e.CreatedAtUtc >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(e => e.CreatedAtUtc < request.To.Value.AddDays(1));

        query = query.WhereBranchAccessible(accessibleBranches);
        if (request.BranchId.HasValue)
            query = query.Where(e => e.Branch.BranchId == request.BranchId.Value);

        var counts = await query
            .GroupBy(e => e.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var pending = counts.Where(c => c.Status == ExaminationStatus.Pending).Sum(c => c.Count);
        var inProgress = counts.Where(c => c.Status == ExaminationStatus.InProgress).Sum(c => c.Count);
        var completed = counts
            .Where(c => c.Status == ExaminationStatus.Completed || c.Status == ExaminationStatus.Delivered)
            .Sum(c => c.Count);

        return new ExaminationCountsByStatusDto(pending, inProgress, completed);
    }
}
