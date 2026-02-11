using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.Branches.Entities;
using MediatR;

namespace Cashif.Application.Branches.Commands.Update;

public class UpdateBranchHandler(IRepository<Branch> repo, IUnitOfWork uow) : IRequestHandler<UpdateBranchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result<bool>.Fail("Not found");

        entity.Update(request.Request.NameAr, request.Request.NameEn);
        if (request.Request.IsActive is not null)
        {
            if (request.Request.IsActive.Value) entity.Activate();
            else entity.Deactivate();
        }

        await repo.UpdateAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}
