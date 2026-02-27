using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetExteriorBodyStage;

public sealed record GetExteriorBodyStageQuery(Guid ExaminationId) : IRequest<ExteriorBodyStageResultDto?>;
