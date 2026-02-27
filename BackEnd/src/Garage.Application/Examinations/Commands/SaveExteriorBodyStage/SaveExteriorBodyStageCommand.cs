using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveExteriorBodyStage;

public sealed record SaveExteriorBodyStageCommand(Guid ExaminationId, SaveExteriorBodyStageRequest Request)
    : IRequest<Result<Guid>>;
