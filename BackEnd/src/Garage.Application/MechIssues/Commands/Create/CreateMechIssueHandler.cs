using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechIssues.Entities;

namespace Garage.Application.MechIssues.Commands.Create
{
    public class CreateMechIssueHandler : BaseCommandHandler<CreateMechIssueCommand, Guid>
    {
        private readonly IRepository<MechIssue> _repo;
        private readonly IUnitOfWork _uow;

        public CreateMechIssueHandler(IRepository<MechIssue> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public override async Task<Result<Guid>> Handle(CreateMechIssueCommand request, CancellationToken ct)
        {
            var req = request.Request;
            var entity = new MechIssue(req.NameAr, req.NameEn, req.MechIssueTypeId);
            await _repo.AddAsync(entity, ct);
            await _uow.SaveChangesAsync(ct);
            return Ok(entity.Id);
        }
    }
}

