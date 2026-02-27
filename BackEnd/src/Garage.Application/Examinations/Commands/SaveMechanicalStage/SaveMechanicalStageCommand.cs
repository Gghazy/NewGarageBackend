using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveMechanicalStage;

public sealed record SaveMechanicalStageCommand(Guid ExaminationId, SaveMechanicalStageRequest Request)
    : IRequest<Result<Guid>>;
