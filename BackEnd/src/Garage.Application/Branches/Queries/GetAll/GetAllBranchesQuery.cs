using Garage.Contracts.Branches;
using MediatR;


namespace Garage.Application.Branches.Queries.GetAll;

public record GetAllBranchesQuery() : IRequest<List<BranchDto>>;
