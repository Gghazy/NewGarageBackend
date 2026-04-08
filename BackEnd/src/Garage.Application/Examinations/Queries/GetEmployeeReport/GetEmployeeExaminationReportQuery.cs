using Garage.Contracts.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetEmployeeReport;

public sealed record GetEmployeeExaminationReportQuery(
    DateTime? DateFrom,
    DateTime? DateTo,
    Guid? BranchId
) : IRequest<List<EmployeeExaminationReportDto>>;
