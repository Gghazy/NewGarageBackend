using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechIssues.Entities;

namespace Garage.Application.MechIssues.Commands.Update
{
    public class UpdateMechIssueHandler : BaseCommandHandler<UpdateMechIssueCommand, bool>
    {
        private readonly IRepository<MechIssue> _repo;
        private readonly IUnitOfWork _uow;

        public UpdateMechIssueHandler(IRepository<MechIssue> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public override async Task<Result<bool>> Handle(UpdateMechIssueCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.Id, ct);
            if (entity is null) return Fail(NotFoundError);

            entity.Update(request.Request.NameAr, request.Request.NameEn, request.Request.MechIssueTypeId);

            await _repo.UpdateAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return Ok(true);
        }
    }
}

