using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveInteriorBodyStage;

public sealed record SaveInteriorBodyStageCommand(Guid ExaminationId, SaveInteriorBodyStageRequest Request)
    : IRequest<Result<Guid>>;
