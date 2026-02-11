using Cashif.Contracts.Common;
using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.Lookup.Queries.GetAllPagination
{
    public record GetAllPaginationQuery<TEntity>(SearchCriteria Search) : IRequest<QueryResult<LookupDto>> where TEntity : LookupBase;

}
