using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetAccessoryStage;

public sealed record GetAccessoryStageQuery(Guid ExaminationId) : IRequest<AccessoryStageResultDto?>;
