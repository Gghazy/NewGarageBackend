using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using MediatR;
namespace Garage.Application.Branches.Queries.GetAll;
public record GetBranchQuery(SearchCriteria Search) : IRequest<QueryResult<BranchDto>>;

