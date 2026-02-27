using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetRoadTestStage;

public sealed record GetRoadTestStageQuery(Guid ExaminationId) : IRequest<RoadTestStageResultDto?>;
