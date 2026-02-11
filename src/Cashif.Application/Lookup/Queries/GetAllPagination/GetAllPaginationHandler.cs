using Cashif.Application.Abstractions;
using Cashif.Application.Lookup.Queries.GetAll;
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

    public sealed class GetAllPaginationHandler<TEntity>: IRequestHandler<GetAllPaginationQuery<TEntity>, QueryResult<LookupDto>> where TEntity : LookupBase
    {
        private readonly ILookupRepository<TEntity> _repo;
        public GetAllPaginationHandler(ILookupRepository<TEntity> repo) => _repo = repo;

        public async Task<QueryResult<LookupDto>> Handle(GetAllPaginationQuery<TEntity> request, CancellationToken ct)
        {
           var res= await _repo.GetAllAsync(request.Search, ct);

            return new QueryResult<LookupDto>
            (
                items: res.Items.Select(x => new LookupDto ( x.Id, x.NameAr, x.NameEn )),
                totalCount: res.TotalCount,
                pageNumber: res.PageNumber,
                pageSize:res.PageSize

            );
        }
             
    }

}
