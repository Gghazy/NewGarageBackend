using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.RoadTestIssues.Entity;

namespace Garage.Application.RoadTestIssues.Commands.Delete;

public sealed class DeleteRoadTestIssueHandler : BaseCommandHandler<DeleteRoadTestIssueCommand, bool>
{
    private readonly IRepository<RoadTestIssue> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteRoadTestIssueHandler(IRepository<RoadTestIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteRoadTestIssueCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
