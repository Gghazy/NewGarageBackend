using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechParts.Entities;

namespace Garage.Application.MechParts.Commands.Update
{
    public class UpdateMechPartHandler : BaseCommandHandler<UpdateMechPartCommand, bool>
    {
        private readonly IRepository<MechPart> _repo;
        private readonly IUnitOfWork _uow;

        public UpdateMechPartHandler(IRepository<MechPart> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public override async Task<Result<bool>> Handle(UpdateMechPartCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.Id, ct);
            if (entity is null) return Fail(NotFoundError);

            entity.Update(request.Request.NameAr, request.Request.NameEn, request.Request.MechPartTypeId);

            await _repo.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return Ok(true);
        }
    }
}
