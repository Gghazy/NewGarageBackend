using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.SaveAccessoryStage;

public sealed record SaveAccessoryStageCommand(Guid ExaminationId, SaveAccessoryStageRequest Request)
    : IRequest<Result<Guid>>;
