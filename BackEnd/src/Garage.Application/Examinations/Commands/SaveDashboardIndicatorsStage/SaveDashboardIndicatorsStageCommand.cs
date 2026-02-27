using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveDashboardIndicatorsStage;

public sealed record SaveDashboardIndicatorsStageCommand(Guid ExaminationId, SaveDashboardIndicatorsStageRequest Request)
    : IRequest<Result<Guid>>;
