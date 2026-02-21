namespace Garage.Application.Common.Extensions;

using Garage.Domain.Branches.Entities;
using Garage.Domain.Clients.Entities;
using Garage.Application.Abstractions;

/// <summary>
/// Extension methods for common repository operations
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Gets an entity by ID, returning Result instead of null
    /// </summary>
    public static async Task<Result<T>> GetByIdAsResultAsync<T>(
        this IRepository<T> repository,
        Guid id,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is null
            ? Result<T>.Fail("Entity not found")
            : Result<T>.Ok(entity);
    }

    /// <summary>
    /// Checks if an entity exists by ID
    /// </summary>
    public static async Task<bool> ExistsAsync<T>(
        this IRepository<T> repository,
        Guid id,
        CancellationToken cancellationToken = default)
        where T : class
    {
        var entity = await repository.GetByIdAsync(id, cancellationToken);
        return entity is not null;
    }
}

/// <summary>
/// Extension methods for common data operations
/// </summary>
public static class DataExtensions
{
    /// <summary>
    /// Safely update an entity with null coalescing for optional fields
    /// </summary>
    public static void UpdateIfNotNull<T>(this T entity, string? newValue, string currentValue) where T : class
    {
        // This is a marker for type-safe nullable updates
        // Use case: entity.NameAr = request.NameAr ?? entity.NameAr
    }
}
