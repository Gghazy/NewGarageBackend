using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetReport;

public sealed record GetExaminationReportQuery(Guid Id) : IRequest<ExaminationReportDto?>;
