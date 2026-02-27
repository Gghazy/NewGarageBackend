using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveRoadTestStage;

public sealed record SaveRoadTestStageCommand(Guid ExaminationId, SaveRoadTestStageRequest Request)
    : IRequest<Result<Guid>>;
