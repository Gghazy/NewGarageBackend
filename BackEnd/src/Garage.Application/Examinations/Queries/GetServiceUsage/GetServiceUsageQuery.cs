using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Domain.ExaminationManagement.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetServiceUsage;

public sealed record ServiceUsageDto(Guid ServiceId, string NameAr, string NameEn, int Count);

public sealed record GetServiceUsageQuery(DateTime? From, DateTime? To, Guid? BranchId)
    : IRequest<List<ServiceUsageDto>>;

public sealed class GetServiceUsageHandler(
    IApplicationDbContext db,
    IBranchAccessService branchAccess)
    : IRequestHandler<GetServiceUsageQuery, List<ServiceUsageDto>>
{
    public async Task<List<ServiceUsageDto>> Handle(GetServiceUsageQuery request, CancellationToken ct)
    {
        var accessibleBranches = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = db.Examinations
            .Where(e => !e.IsDeleted && e.Status != ExaminationStatus.Cancelled);

        if (request.From.HasValue)
            query = query.Where(e => e.CreatedAtUtc >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(e => e.CreatedAtUtc < request.To.Value.AddDays(1));

        query = query.WhereBranchAccessible(accessibleBranches);
        if (request.BranchId.HasValue)
            query = query.Where(e => e.Branch.BranchId == request.BranchId.Value);

        // EF Core can't translate GroupBy on owned type (ServiceSnapshot) to SQL,
        // so we load the flat columns first, then group in memory.
        var items = await query
            .SelectMany(e => e.Items)
            .Select(i => new { i.Service.ServiceId, i.Service.NameAr, i.Service.NameEn })
            .ToListAsync(ct);

        return items
            .GroupBy(x => new { x.ServiceId, x.NameAr, x.NameEn })
            .Select(g => new ServiceUsageDto(g.Key.ServiceId, g.Key.NameAr, g.Key.NameEn, g.Count()))
            .OrderByDescending(x => x.Count)
            .ToList();
    }
}
