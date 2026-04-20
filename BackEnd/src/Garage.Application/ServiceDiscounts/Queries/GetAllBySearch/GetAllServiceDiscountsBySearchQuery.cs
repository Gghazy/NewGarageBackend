using Garage.Contracts.Common;
using Garage.Contracts.ServiceDiscounts;
using MediatR;

namespace Garage.Application.ServiceDiscounts.Queries.GetAllBySearch;

public sealed record GetAllServiceDiscountsBySearchQuery(ServiceDiscountFilterDto Request) : IRequest<QueryResult<ServiceDiscountDto>>;
