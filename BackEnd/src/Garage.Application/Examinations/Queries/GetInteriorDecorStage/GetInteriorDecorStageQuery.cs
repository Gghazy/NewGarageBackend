using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetInteriorDecorStage;

public sealed record GetInteriorDecorStageQuery(Guid ExaminationId) : IRequest<InteriorDecorStageResultDto?>;
