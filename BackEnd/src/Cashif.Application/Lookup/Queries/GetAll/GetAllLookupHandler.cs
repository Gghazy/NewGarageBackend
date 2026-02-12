using Cashif.Application.Abstractions;
using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.Lookup.Queries.GetAll
{
    public sealed class GetAllLookupHandler<TEntity>
     : IRequestHandler<GetAllLookupQuery<TEntity>, List<LookupDto>>
     where TEntity : LookupBase
    {
        private readonly ILookupRepository<TEntity> _repo;
        public GetAllLookupHandler(ILookupRepository<TEntity> repo) => _repo = repo;

        public async Task<List<LookupDto>> Handle(GetAllLookupQuery<TEntity> request, CancellationToken ct)
            => (await _repo.GetAllAsync(ct))
                .Select(x => new LookupDto(x.Id, x.NameAr, x.NameEn))
                .ToList();
    }

}
