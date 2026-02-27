using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveInteriorDecorStage;

public sealed record SaveInteriorDecorStageCommand(Guid ExaminationId, SaveInteriorDecorStageRequest Request)
    : IRequest<Result<Guid>>;
