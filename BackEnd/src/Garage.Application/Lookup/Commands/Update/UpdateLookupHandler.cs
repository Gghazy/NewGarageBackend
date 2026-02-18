using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Domain.Common.Primitives;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Lookup.Commands.Update
{
    public sealed class UpdateLookupHandler<TEntity>
     : IRequestHandler<UpdateLookupCommand<TEntity>, bool>
     where TEntity : LookupBase
    {
        private readonly ILookupRepository<TEntity> _repo;
        private readonly IUnitOfWork _uow;

        public UpdateLookupHandler(ILookupRepository<TEntity> repo, IUnitOfWork uow)
        {
            _repo = repo; _uow = uow;
        }

        public async Task<bool> Handle(UpdateLookupCommand<TEntity> request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.Id, ct);
            if (entity is null) return false;

            entity.Update(request.Req.NameAr, request.Req.NameEn);

            await _uow.SaveChangesAsync(ct);
            return true;
        }
    }

}

