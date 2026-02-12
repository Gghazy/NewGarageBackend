using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.SensorIssues.Entities;
using MediatR;

namespace Cashif.Application.SensorIssues.Commands.Update;

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
