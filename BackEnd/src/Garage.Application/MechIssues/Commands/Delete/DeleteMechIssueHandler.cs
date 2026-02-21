using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechIssues.Entities;

namespace Garage.Application.MechIssues.Commands.Delete;

public sealed class DeleteMechIssueHandler : BaseCommandHandler<DeleteMechIssueCommand, bool>
{
    private readonly IRepository<MechIssue> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteMechIssueHandler(IRepository<MechIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteMechIssueCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
