using System.Linq.Expressions;
namespace Garage.Application.Abstractions;
public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(CancellationToken ct = default);
    IQueryable<T> Query();
    IQueryable<T> QueryTracking();
    
    /// <summary>
    /// Query that includes soft-deleted records (for admin views, audits, etc.)
    /// </summary>
    IQueryable<T> QueryIncludingDeleted();
    
    /// <summary>
    /// Tracking query that includes soft-deleted records
    /// </summary>
    IQueryable<T> QueryTrackingIncludingDeleted();
}

