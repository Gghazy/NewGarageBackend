using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetMechanicalStage;

public sealed record GetMechanicalStageQuery(Guid ExaminationId) : IRequest<MechanicalStageResultDto?>;
