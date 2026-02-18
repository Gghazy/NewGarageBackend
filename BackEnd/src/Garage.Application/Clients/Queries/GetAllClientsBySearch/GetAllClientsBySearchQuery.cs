using Garage.Contracts.Branches;
using Garage.Contracts.Clients;
using Garage.Contracts.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Clients.Queries.GetAllClientsBySearch;

public record GetAllClientsBySearchQuery(SearchCriteria Search) : IRequest<QueryResult<ClientDto>>;
