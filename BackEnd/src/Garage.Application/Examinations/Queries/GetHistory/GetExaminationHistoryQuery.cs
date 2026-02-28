using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetHistory;

public sealed record GetExaminationHistoryQuery(Guid ExaminationId)
    : IRequest<List<ExaminationHistoryDto>>;
