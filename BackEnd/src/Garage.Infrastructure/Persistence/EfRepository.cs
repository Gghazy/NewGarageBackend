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
        var entity = await _db.Set<T>().FindAsync(new object?[] { id }, ct);
        
        // Check if entity is soft-deleted (only if T inherits from Entity)
        if (entity is Entity softDeleteEntity && softDeleteEntity.IsDeleted)
            return null;
        
        return entity;
    }

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await ApplySoftDeleteFilter(_db.Set<T>()).AnyAsync(predicate, ct);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default)
        => await ApplySoftDeleteFilter(_db.Set<T>())
            .AsNoTracking()
            .ToListAsync(ct);

    public IQueryable<T> Query() => ApplySoftDeleteFilter(_db.Set<T>()).AsNoTracking().AsQueryable();

    public IQueryable<T> QueryTracking() => ApplySoftDeleteFilter(_db.Set<T>()).AsQueryable();

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

    /// <summary>
    /// Soft delete: marks entity as deleted without removing from database
    /// </summary>
    public async Task SoftDeleteAsync(T entity, Guid? deletedBy = null, CancellationToken ct = default)
    {
        if (entity is Entity softDeleteEntity)
        {
            softDeleteEntity.SoftDelete(deletedBy);
            _db.Set<T>().Update(entity);
        }
        else
        {
            // Fallback to hard delete if entity doesn't support soft delete
            _db.Set<T>().Remove(entity);
        }
    }

    /// <summary>
    /// Hard delete: permanently removes entity from database
    /// </summary>
    public Task HardDeleteAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    public async Task RestoreAsync(T entity, CancellationToken ct = default)
    {
        if (entity is Entity softDeleteEntity)
        {
            softDeleteEntity.Restore();
            _db.Set<T>().Update(entity);
        }
    }

    /// <summary>
    /// Apply soft delete filter to automatically exclude deleted records
    /// </summary>
    private IQueryable<T> ApplySoftDeleteFilter(IQueryable<T> query)
    {
        // Only apply filter if T is an Entity (has soft delete support)
        if (typeof(Entity).IsAssignableFrom(typeof(T)))
        {
            // Cast to Entity and filter out deleted records
            return query.Where(e => !(e as Entity)!.IsDeleted);
        }

        return query;
    }

    /// <summary>
    /// Include soft-deleted records in query (for admin views, audits, etc.)
    /// </summary>
    public IQueryable<T> QueryIncludingDeleted() 
        => _db.Set<T>().AsNoTracking().AsQueryable();

    /// <summary>
    /// Include soft-deleted records in tracking query
    /// </summary>
    public IQueryable<T> QueryTrackingIncludingDeleted() 
        => _db.Set<T>().AsQueryable();
}

