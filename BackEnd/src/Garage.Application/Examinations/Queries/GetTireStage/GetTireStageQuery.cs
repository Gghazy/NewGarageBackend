using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetTireStage;

public sealed record GetTireStageQuery(Guid ExaminationId) : IRequest<TireStageResultDto?>;
