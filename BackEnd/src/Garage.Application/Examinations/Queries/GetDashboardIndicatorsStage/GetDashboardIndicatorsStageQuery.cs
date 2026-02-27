using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetDashboardIndicatorsStage;

public sealed record GetDashboardIndicatorsStageQuery(Guid ExaminationId) : IRequest<DashboardIndicatorsStageResultDto?>;
