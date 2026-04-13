using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Employees.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetEmployeeComparison;

// ── DTOs ────────────────────────────────────────────────────────────────

public sealed record EmployeePeriodDto(
    Guid EmployeeId,
    Guid UserId,
    string NameAr,
    string NameEn,
    int ExaminationCount,
    int TotalStageActions
);

public sealed record EmployeePeriodSummaryDto(
    List<EmployeePeriodDto> Employees,
    int TotalExaminations,
    int TotalStageActions
);

public sealed record EmployeeComparisonResponse(
    EmployeePeriodSummaryDto Period1,
    EmployeePeriodSummaryDto Period2
);

// ── Query ───────────────────────────────────────────────────────────────

public sealed record GetEmployeeComparisonQuery(
    DateTime From1,
    DateTime To1,
    DateTime From2,
    DateTime To2,
    Guid? BranchId
) : IRequest<EmployeeComparisonResponse>;

// ── Handler ─────────────────────────────────────────────────────────────

public sealed class GetEmployeeComparisonHandler(
    IReadRepository<ExaminationHistory> historyRepo,
    IReadRepository<Employee> employeeRepo,
    IBranchAccessService branchAccess)
    : IRequestHandler<GetEmployeeComparisonQuery, EmployeeComparisonResponse>
{
    private static readonly ExaminationHistoryAction[] StageActions =
    {
        ExaminationHistoryAction.SensorStageSaved,
        ExaminationHistoryAction.DashboardIndicatorsStageSaved,
        ExaminationHistoryAction.InteriorDecorStageSaved,
        ExaminationHistoryAction.InteriorBodyStageSaved,
        ExaminationHistoryAction.ExteriorBodyStageSaved,
        ExaminationHistoryAction.TireStageSaved,
        ExaminationHistoryAction.AccessoryStageSaved,
        ExaminationHistoryAction.MechanicalStageSaved,
        ExaminationHistoryAction.RoadTestStageSaved,
    };

    public async Task<EmployeeComparisonResponse> Handle(
        GetEmployeeComparisonQuery request, CancellationToken ct)
    {
        var branchIds = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var period1 = await BuildPeriod(request.From1, request.To1, request.BranchId, ct);
        var period2 = await BuildPeriod(request.From2, request.To2, request.BranchId, ct);

        return new EmployeeComparisonResponse(period1, period2);
    }

    private async Task<EmployeePeriodSummaryDto> BuildPeriod(
        DateTime from, DateTime to, Guid? branchId, CancellationToken ct)
    {
        var query = historyRepo.Query()
            .Where(h => StageActions.Contains(h.Action) && h.CreatedBy.HasValue)
            .Where(h => h.CreatedAtUtc >= from.Date)
            .Where(h => h.CreatedAtUtc < to.Date.AddDays(1));

        var historyData = await query
            .Select(h => new { h.CreatedBy, h.ExaminationId })
            .ToListAsync(ct);

        if (historyData.Count == 0)
            return new EmployeePeriodSummaryDto(new List<EmployeePeriodDto>(), 0, 0);

        var userIds = historyData.Select(h => h.CreatedBy!.Value).Distinct().ToList();

        var employees = await employeeRepo.Query()
            .Where(e => userIds.Contains(e.UserId))
            .Select(e => new { e.Id, e.UserId, e.NameAr, e.NameEn })
            .ToListAsync(ct);

        var employeeByUserId = employees.ToDictionary(e => e.UserId);

        var result = historyData
            .Where(h => employeeByUserId.ContainsKey(h.CreatedBy!.Value))
            .GroupBy(h => h.CreatedBy!.Value)
            .Select(g =>
            {
                var emp = employeeByUserId[g.Key];
                return new EmployeePeriodDto(
                    emp.Id, emp.UserId, emp.NameAr, emp.NameEn,
                    g.Select(h => h.ExaminationId).Distinct().Count(),
                    g.Count());
            })
            .OrderByDescending(e => e.ExaminationCount)
            .ToList();

        return new EmployeePeriodSummaryDto(
            result,
            result.Sum(e => e.ExaminationCount),
            result.Sum(e => e.TotalStageActions));
    }
}
