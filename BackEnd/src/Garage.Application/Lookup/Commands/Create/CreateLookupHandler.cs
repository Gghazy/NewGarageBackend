using Garage.Application.Abstractions;
using Garage.Domain.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Lookup.Commands.Create
{
    public sealed class CreateLookupHandler<TEntity>
     : IRequestHandler<CreateLookupCommand<TEntity>, Guid>
     where TEntity : LookupBase
    {
        private readonly ILookupRepository<TEntity> _repo;
        private readonly IUnitOfWork _uow;

        public CreateLookupHandler(ILookupRepository<TEntity> repo, IUnitOfWork uow)
        {
            _repo = repo; _uow = uow;
        }

        public async Task<Guid> Handle(CreateLookupCommand<TEntity> request, CancellationToken ct)
        {
            // ???? ?? entity ???? ???? ctor (string ar,string en,bool active)
            var entity = (TEntity)Activator.CreateInstance(
                typeof(TEntity),
                request.Req.NameAr,
                request.Req.NameEn
            )!;

            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);

            return entity.Id;
        }
    }

}

