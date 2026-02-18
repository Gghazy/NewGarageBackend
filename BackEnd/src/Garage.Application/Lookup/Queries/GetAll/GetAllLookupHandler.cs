using Garage.Application.Abstractions.Repositories;
using Garage.Contracts.Lookup;
using Garage.Domain.Common.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Lookup.Queries.GetAll
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

