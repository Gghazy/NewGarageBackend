using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Domain.Common.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Lookup.Queries.GetAllPagination
{
    public record GetAllPaginationQuery<TEntity>(SearchCriteria Search) : IRequest<QueryResult<LookupDto>> where TEntity : LookupBase;

}

