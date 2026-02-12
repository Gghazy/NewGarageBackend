using Cashif.Contracts.Common;
using Cashif.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Application.Abstractions
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
