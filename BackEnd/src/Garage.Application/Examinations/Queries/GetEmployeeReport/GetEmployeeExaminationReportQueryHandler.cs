using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Examinations;
using Garage.Domain.Employees.Entities;
using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Examinations.Queries.GetEmployeeReport;

public sealed class GetEmployeeExaminationReportQueryHandler(
    IReadRepository<ExaminationHistory> historyRepo,
    IReadRepository<Employee> employeeRepo,
    IBranchAccessService branchAccess)
    : BaseQueryHandler<GetEmployeeExaminationReportQuery, List<EmployeeExaminationReportDto>>
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

    public override async Task<List<EmployeeExaminationReportDto>> Handle(
        GetEmployeeExaminationReportQuery request, CancellationToken ct)
    {
        var branchIds = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = historyRepo.Query()
            .Where(h => StageActions.Contains(h.Action) && h.CreatedBy.HasValue);

        if (request.DateFrom.HasValue)
            query = query.Where(h => h.CreatedAtUtc >= request.DateFrom.Value.Date);
        if (request.DateTo.HasValue)
            query = query.Where(h => h.CreatedAtUtc < request.DateTo.Value.Date.AddDays(1));

        var historyData = await query
            .Select(h => new
            {
                h.CreatedBy,
                h.ExaminationId,
                h.Action,
            })
            .ToListAsync(ct);

        if (historyData.Count == 0)
            return new List<EmployeeExaminationReportDto>();

        var userIds = historyData
            .Select(h => h.CreatedBy!.Value)
            .Distinct()
            .ToList();

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
                var examCount = g.Select(h => h.ExaminationId).Distinct().Count();
                var stageCounts = g
                    .GroupBy(h => h.Action.ToString())
                    .Select(sg => new StageCountDto(sg.Key, sg.Count()))
                    .OrderBy(s => s.Stage)
                    .ToList();

                return new EmployeeExaminationReportDto(
                    emp.Id,
                    emp.UserId,
                    emp.NameAr,
                    emp.NameEn,
                    examCount,
                    g.Count(),
                    stageCounts
                );
            })
            .OrderByDescending(e => e.ExaminationCount)
            .ToList();

        return result;
    }
}
