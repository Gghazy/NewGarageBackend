using Azure.Core;
using Cashif.Application.Abstractions;
using Cashif.Application.Common.Extensions;
using Cashif.Contracts.Common;
using Cashif.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Infrastructure.Persistence
{
    public class LookupRepository<TEntity> : ILookupRepository<TEntity>
      where TEntity : LookupBase
    {
        private readonly ApplicationDbContext _db;
        public LookupRepository(ApplicationDbContext db) => _db = db;

        public async Task<List<TEntity>> GetAllAsync(CancellationToken ct)
        {
            var q = _db.Set<TEntity>().AsNoTracking().AsQueryable();
            return await q.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
        }

        public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id, ct);

        public Task AddAsync(TEntity entity, CancellationToken ct)
            => _db.Set<TEntity>().AddAsync(entity, ct).AsTask();

        public void Remove(TEntity entity) => _db.Set<TEntity>().Remove(entity);


        public async Task<QueryResult<TEntity>> GetAllAsync(SearchCriteria search, CancellationToken ct)
        {
            var q = _db.Set<TEntity>().AsNoTracking().AsQueryable();
            return await q.OrderByDescending(x => x.Id)
                .ToQueryResult(search.CurrentPage, search.ItemsPerPage);
            ;
        }
    }

}
