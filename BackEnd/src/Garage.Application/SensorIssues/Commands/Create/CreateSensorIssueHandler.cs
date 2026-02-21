using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.SensorIssues.Entities;
namespace Garage.Application.SensorIssues.Commands.Create;
public class CreateSensorIssueHandler : BaseCommandHandler<CreateSensorIssueCommand, Guid>
{
    private readonly IRepository<SensorIssue> _repo;
    private readonly IUnitOfWork _uow;

    public CreateSensorIssueHandler(IRepository<SensorIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(CreateSensorIssueCommand request, CancellationToken ct)              
    {
        var req = request.Request;
        var entity = new SensorIssue(req.NameAr, req.NameEn, req.code);
        await _repo.AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(entity.Id);
    }
}

