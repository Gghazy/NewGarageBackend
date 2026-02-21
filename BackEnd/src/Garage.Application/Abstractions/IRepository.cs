namespace Garage.Application.Abstractions;
public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    
    /// <summary>
    /// Soft delete: marks entity as deleted without removing from database
    /// </summary>
    Task SoftDeleteAsync(T entity, Guid? deletedBy = null, CancellationToken ct = default);
    
    /// <summary>
    /// Hard delete: permanently removes entity from database
    /// </summary>
    Task HardDeleteAsync(T entity, CancellationToken ct = default);
    
    /// <summary>
    /// Restore a soft-deleted entity
    /// </summary>
    Task RestoreAsync(T entity, CancellationToken ct = default);
}

