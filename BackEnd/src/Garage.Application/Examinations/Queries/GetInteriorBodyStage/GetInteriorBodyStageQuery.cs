using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetInteriorBodyStage;

public sealed record GetInteriorBodyStageQuery(Guid ExaminationId) : IRequest<InteriorBodyStageResultDto?>;
