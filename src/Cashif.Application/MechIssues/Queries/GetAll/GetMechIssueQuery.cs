using Cashif.Contracts.Common;
using Cashif.Contracts.MechIssues;
using MediatR;


namespace Cashif.Application.MechIssues.Queries.GetAll
{
    public record GetMechIssueQuery(SearchCriteria Search) : IRequest<QueryResult<MechIssueResponse>>;

}
