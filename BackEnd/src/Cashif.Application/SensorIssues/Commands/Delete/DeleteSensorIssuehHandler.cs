using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.SensorIssues.Entities;
using MediatR;

namespace Cashif.Application.SensorIssues.Commands.Delete;

public class DeleteSensorIssuehHandler(IRepository<SensorIssue> repo, IUnitOfWork uow) : IRequestHandler<DeleteSensorIssueCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSensorIssueCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result<bool>.Fail("Not found");
        await repo.RemoveAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}
