using Cashif.Contracts.Branches;
using Cashif.Contracts.Common;
using MediatR;
namespace Cashif.Application.Branches.Queries.GetAll;
public record GetBranchQuery(SearchCriteria Search) : IRequest<QueryResult<BranchDto>>;
