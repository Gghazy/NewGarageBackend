using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetSensorStage;

public sealed record GetSensorStageQuery(Guid ExaminationId) : IRequest<SensorStageResultDto?>;
