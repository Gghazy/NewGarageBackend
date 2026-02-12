using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Domain.SensorIssues.Entities;
using MediatR;

namespace Garage.Application.SensorIssues.Commands.Update;

public class UpdateSensorIssueHandler(IRepository<SensorIssue> repo, IUnitOfWork uow) : IRequestHandler<UpdateSensorIssueCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateSensorIssueCommand command, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(command.Id, ct);
        if (entity is null) return Result<bool>.Fail("Not found");

        entity.Update(command.Request.NameAr, command.Request.NameEn, command.Request.code);

        await repo.UpdateAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}

