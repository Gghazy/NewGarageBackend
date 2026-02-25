using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetById;

public sealed record GetExaminationByIdQuery(Guid Id) : IRequest<ExaminationDto?>;
