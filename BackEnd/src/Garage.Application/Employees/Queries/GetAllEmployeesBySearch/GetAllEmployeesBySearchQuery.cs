

using Garage.Contracts.Common;
using Garage.Contracts.Employees;
using MediatR;

namespace Garage.Application.Employees.Queries.GetAllEmployeesBySearch;


public record GetAllEmployeesBySearchQuery(SearchCriteria Search) : IRequest<QueryResult<EmployeeDto>>;
