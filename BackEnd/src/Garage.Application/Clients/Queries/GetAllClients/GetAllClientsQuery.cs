using Garage.Contracts.Branches;
using Garage.Contracts.Lookup;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Clients.Queries.GetAllClients;

public record GetAllClientsQuery() : IRequest<List<LookupDto>>;


