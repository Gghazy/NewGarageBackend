using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.RoadTestIssues.Entity;

namespace Garage.Application.RoadTestIssues.Commands.Update;

public class UpdateRoadTestIssueHandler : BaseCommandHandler<UpdateRoadTestIssueCommand, bool>
{
    private readonly IRepository<RoadTestIssue> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateRoadTestIssueHandler(IRepository<RoadTestIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(UpdateRoadTestIssueCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Fail(NotFoundError);

        entity.Update(request.Request.NameAr, request.Request.NameEn, request.Request.RoadTestIssueTypeId);

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(true);
    }
}
