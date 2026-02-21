using Garage.Application.Common;
using Garage.Domain.Common.Primitives;
using MediatR;

namespace Garage.Application.Lookup.Commands.Delete
{
    public record DeleteLookupCommand<TEntity>(Guid Id)
        : IRequest<Result<bool>>
        where TEntity : LookupBase;
}
