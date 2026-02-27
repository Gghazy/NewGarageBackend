using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.RoadTestIssues.Entity;

namespace Garage.Application.RoadTestIssues.Commands.Create;

public class CreateRoadTestIssueHandler : BaseCommandHandler<CreateRoadTestIssueCommand, Guid>
{
    private readonly IRepository<RoadTestIssue> _repo;
    private readonly IUnitOfWork _uow;

    public CreateRoadTestIssueHandler(IRepository<RoadTestIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(CreateRoadTestIssueCommand request, CancellationToken ct)
    {
        var req = request.Request;
        var entity = new RoadTestIssue(req.NameAr, req.NameEn, req.RoadTestIssueTypeId);
        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(entity.Id);
    }
}
