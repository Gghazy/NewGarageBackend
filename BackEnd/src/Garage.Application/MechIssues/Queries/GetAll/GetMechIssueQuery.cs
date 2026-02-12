using Garage.Contracts.Common;
using Garage.Contracts.MechIssues;
using MediatR;


namespace Garage.Application.MechIssues.Queries.GetAll
{
    public record GetMechIssueQuery(SearchCriteria Search) : IRequest<QueryResult<MechIssueResponse>>;

}

