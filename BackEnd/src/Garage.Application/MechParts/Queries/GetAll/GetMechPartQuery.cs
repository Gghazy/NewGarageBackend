using Garage.Contracts.Common;
using Garage.Contracts.MechParts;
using MediatR;


namespace Garage.Application.MechParts.Queries.GetAll
{
    public record GetMechPartQuery(SearchCriteria Search) : IRequest<QueryResult<MechPartResponse>>;

}
