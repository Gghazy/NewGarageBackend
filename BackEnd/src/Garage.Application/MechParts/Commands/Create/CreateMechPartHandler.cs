using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechParts.Entities;

namespace Garage.Application.MechParts.Commands.Create
{
    public class CreateMechPartHandler : BaseCommandHandler<CreateMechPartCommand, Guid>
    {
        private readonly IRepository<MechPart> _repo;
        private readonly IUnitOfWork _uow;

        public CreateMechPartHandler(IRepository<MechPart> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public override async Task<Result<Guid>> Handle(CreateMechPartCommand request, CancellationToken ct)
        {
            var req = request.Request;
            var entity = new MechPart(req.NameAr, req.NameEn, req.MechPartTypeId);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return Ok(entity.Id);
        }
    }
}
