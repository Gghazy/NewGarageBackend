using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using Garage.Contracts.Services;
using MediatR;


namespace Garage.Application.Services.Queries.GetAllPagination
{
    public record GetAllServiceBySearchQuery(SearchCriteria Search) : IRequest<QueryResult<ServiceDto>>;

}
