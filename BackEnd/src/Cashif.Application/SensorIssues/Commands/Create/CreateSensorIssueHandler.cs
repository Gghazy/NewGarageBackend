using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.SensorIssues.Entities;
using MediatR;
namespace Cashif.Application.SensorIssues.Commands.Create;
public class CreateSensorIssueHandler(IRepository<SensorIssue> repo, IUnitOfWork uow) : IRequestHandler<CreateSensorIssueCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSensorIssueCommand request, CancellationToken ct)              
    {
        var req = request.Request;
        var entity = new SensorIssue(req.NameAr, req.NameEn, req.code);
        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<Guid>.Ok(entity.Id);
    }
}
