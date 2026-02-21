using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.SensorIssues.Entities;

namespace Garage.Application.SensorIssues.Commands.Update;

public class UpdateSensorIssueHandler : BaseCommandHandler<UpdateSensorIssueCommand, bool>
{
    private readonly IRepository<SensorIssue> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateSensorIssueHandler(IRepository<SensorIssue> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(UpdateSensorIssueCommand command, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(command.Id, ct);
        if (entity is null) return Fail(NotFoundError);

        entity.Update(command.Request.NameAr, command.Request.NameEn, command.Request.code);

        await _repo.UpdateAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return Ok(true);
    }
}

