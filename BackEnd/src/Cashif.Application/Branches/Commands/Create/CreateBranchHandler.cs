using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.Branches.Entities;
using MediatR;
namespace Cashif.Application.Branches.Commands.Create;
public class CreateBranchHandler(IRepository<Branch> repo, IUnitOfWork uow) : IRequestHandler<CreateBranchCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBranchCommand request, CancellationToken ct)              
    {
        var req = request.Request;
        var entity = new Branch(req.NameAr, req.NameEn, req.IsActive);
        await repo.AddAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<Guid>.Ok(entity.Id);
    }
}
