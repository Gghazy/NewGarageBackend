namespace Garage.Application.Abstractions;
public interface IRepository<T> : IReadRepository<T> where T : class
{
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task RemoveAsync(T entity, CancellationToken ct = default);
}

