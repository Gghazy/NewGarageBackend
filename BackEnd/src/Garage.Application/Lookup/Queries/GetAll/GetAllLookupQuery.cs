using Garage.Contracts.Lookup;
using Garage.Domain.Common;
using MediatR;

namespace Garage.Application.Lookup.Queries.GetAll
{
    public record GetAllLookupQuery<TEntity>(): IRequest<List<LookupDto>> where TEntity : LookupBase;
}

