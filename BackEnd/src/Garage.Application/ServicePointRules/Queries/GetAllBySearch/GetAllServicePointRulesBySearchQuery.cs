using Garage.Contracts.Common;
using Garage.Contracts.ServicePointRules;
using MediatR;

namespace Garage.Application.ServicePointRules.Queries.GetAllBySearch;

public sealed record GetAllServicePointRulesBySearchQuery(ServicePointRuleFilterDto Request) : IRequest<QueryResult<ServicePointRuleDto>>;
