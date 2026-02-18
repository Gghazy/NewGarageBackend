using Garage.Contracts.Branches;
using Garage.Contracts.Common;
using MediatR;
namespace Garage.Application.Branches.Queries.GetAllBranchesBySearch;
public record GetAllBranchesBySearchQuery(SearchCriteria Search) : IRequest<QueryResult<BranchDto>>;

