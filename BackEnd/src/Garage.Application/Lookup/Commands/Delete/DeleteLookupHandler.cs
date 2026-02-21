using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Common.Primitives;
using MediatR;

namespace Garage.Application.Lookup.Commands.Delete
{
    public sealed class DeleteLookupHandler<TEntity>
        : IRequestHandler<DeleteLookupCommand<TEntity>, Result<bool>>
        where TEntity : LookupBase
    {
        private readonly ILookupRepository<TEntity> _repo;
        private readonly IUnitOfWork _uow;

        public DeleteLookupHandler(ILookupRepository<TEntity> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<Result<bool>> Handle(DeleteLookupCommand<TEntity> request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.Id, ct);
            if (entity is null)
                return Result<bool>.Fail("Common.NotFound");

            await _repo.SoftDeleteAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return Result<bool>.Ok(true);
        }
    }
}
