using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveSensorStage;

public sealed record SaveSensorStageCommand(Guid ExaminationId, SaveSensorStageRequest Request)
    : IRequest<Result<Guid>>;
