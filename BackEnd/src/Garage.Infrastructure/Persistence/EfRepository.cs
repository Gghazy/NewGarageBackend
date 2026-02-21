using Garage.Application.Abstractions;
using Garage.Domain.Common.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Garage.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    public EfRepository(ApplicationDbContext db) => _db = db;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        // FindAsync bypasses global query filters, so we check manually
        var entity = await _db.Set<T>().FindAsync(new object?[] { id }, ct);

        if (entity is Entity softDeleteEntity && softDeleteEntity.IsDeleted)
            return null;

        return entity;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _db.Set<T>().AnyAsync(predicate, ct);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default)
        => await _db.Set<T>().AsNoTracking().ToListAsync(ct);

    // Global query filter (IsDeleted = false) is applied automatically by EF Core
    public IQueryable<T> Query() => _db.Set<T>().AsNoTracking().AsQueryable();

    public IQueryable<T> QueryTracking() => _db.Set<T>().AsQueryable();

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _db.Set<T>().AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Update(entity);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(T entity, Guid? deletedBy = null, CancellationToken ct = default)
    {
        if (entity is Entity softDeleteEntity)
        {
            softDeleteEntity.SoftDelete(deletedBy);
            _db.Set<T>().Update(entity);
        }
        else
        {
            _db.Set<T>().Remove(entity);
        }
    }

    public Task HardDeleteAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    public async Task RestoreAsync(T entity, CancellationToken ct = default)
    {
        if (entity is Entity softDeleteEntity)
        {
            softDeleteEntity.Restore();
            _db.Set<T>().Update(entity);
        }
    }

    // Use IgnoreQueryFilters() to include soft-deleted records (admin/audit use)
    public IQueryable<T> QueryIncludingDeleted()
        => _db.Set<T>().IgnoreQueryFilters().AsNoTracking().AsQueryable();

    public IQueryable<T> QueryTrackingIncludingDeleted()
        => _db.Set<T>().IgnoreQueryFilters().AsQueryable();
}
