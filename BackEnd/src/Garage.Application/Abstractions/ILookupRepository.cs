using Garage.Contracts.Common;
using Garage.Domain.Common.Lookup;


namespace Garage.Application.Abstractions
{
    public interface ILookupRepository<TEntity> where TEntity : LookupBase
    {
        Task<List<TEntity>> GetAllAsync(CancellationToken ct);
        Task<QueryResult<TEntity>> GetAllAsync(SearchCriteria search, CancellationToken ct);
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct);
        Task AddAsync(TEntity entity, CancellationToken ct);
        void Remove(TEntity entity);
    }

}

