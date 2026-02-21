using Azure.Core;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Domain.Common.Primitives;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence
{
    public class LookupRepository<TEntity> : ILookupRepository<TEntity>
      where TEntity : LookupBase
    {
        private readonly ApplicationDbContext _db;
        public LookupRepository(ApplicationDbContext db) => _db = db;

        public async Task<List<TEntity>> GetAllAsync(CancellationToken ct)
        {
            var q = _db.Set<TEntity>().Where(x => !x.IsDeleted).AsNoTracking().AsQueryable();
            return await q.OrderByDescending(x => x.CreatedAtUtc).ToListAsync(ct);
        }

        public Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct)
            => _db.Set<TEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

        public Task AddAsync(TEntity entity, CancellationToken ct)
            => _db.Set<TEntity>().AddAsync(entity, ct).AsTask();

        public void Remove(TEntity entity) => _db.Set<TEntity>().Remove(entity);

        public Task SoftDeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            entity.SoftDelete();
            _db.Set<TEntity>().Update(entity);
            return Task.CompletedTask;
        }

        public async Task<QueryResult<TEntity>> GetAllAsync(SearchCriteria search, CancellationToken ct)
        {
            var q = _db.Set<TEntity>().Where(x => !x.IsDeleted).AsNoTracking().AsQueryable();
            return await q.OrderByDescending(x => x.Id)
                .ToQueryResult(search.CurrentPage, search.ItemsPerPage);
        }
    }

}

