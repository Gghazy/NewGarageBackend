using Cashif.Contracts.Lookup;
using Cashif.Domain.Common;
using MediatR;

namespace Cashif.Application.Lookup.Queries.GetAll
{
    public record GetAllLookupQuery<TEntity>(): IRequest<List<LookupDto>> where TEntity : LookupBase;
}
