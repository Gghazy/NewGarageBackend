using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveTireStage;

public sealed record SaveTireStageCommand(Guid ExaminationId, SaveTireStageRequest Request)
    : IRequest<Result<Guid>>;
