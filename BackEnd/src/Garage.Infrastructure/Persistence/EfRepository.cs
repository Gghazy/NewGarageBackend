using Garage.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Garage.Infrastructure.Persistence;

public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly ApplicationDbContext _db;
    public EfRepository(ApplicationDbContext db) => _db = db;

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Set<T>().FindAsync(new object?[] { id }, ct);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
        => await _db.Set<T>().AnyAsync(predicate, ct);

    public async Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default)
        => await _db.Set<T>().AsNoTracking().ToListAsync(ct);

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

    public Task RemoveAsync(T entity, CancellationToken ct = default)
    {
        _db.Set<T>().Remove(entity);
        return Task.CompletedTask;
    }
}

