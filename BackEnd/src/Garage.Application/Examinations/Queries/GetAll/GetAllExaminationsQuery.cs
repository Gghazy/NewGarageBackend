using Garage.Contracts.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetAll;

public sealed record GetAllExaminationsQuery(SearchCriteria Search) : IRequest<QueryResult<ExaminationDto>>;
