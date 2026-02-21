using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Domain.SensorIssues.Entities;
using MediatR;

namespace Garage.Application.SensorIssues.Commands.Delete;

public class DeleteSensorIssueHandler(IRepository<SensorIssue> repo, IUnitOfWork uow) : IRequestHandler<DeleteSensorIssueCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteSensorIssueCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result<bool>.Fail("Not found");
        await repo.SoftDeleteAsync(entity, ct: ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}

