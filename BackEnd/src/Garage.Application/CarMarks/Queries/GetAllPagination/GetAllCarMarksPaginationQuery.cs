using Garage.Contracts.CarMarks;
using Garage.Contracts.Common;
using MediatR;

namespace Garage.Application.CarMarks.Queries.GetAllPagination;

public sealed record GetAllCarMarksPaginationQuery(SearchCriteria Search) : IRequest<QueryResult<CarMarkDto>>;
