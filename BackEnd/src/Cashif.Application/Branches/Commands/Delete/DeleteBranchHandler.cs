using Cashif.Application.Abstractions;
using Cashif.Application.Common;
using Cashif.Domain.Branches.Entities;
using MediatR;

namespace Cashif.Application.Branches.Commands.Delete;

public class DeleteBranchHandler(IRepository<Branch> repo, IUnitOfWork uow) : IRequestHandler<DeleteBranchCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        var entity = await repo.GetByIdAsync(request.Id, ct);
        if (entity is null) return Result<bool>.Fail("Not found");
        await repo.RemoveAsync(entity, ct);
        await uow.SaveChangesAsync(ct);
        return Result<bool>.Ok(true);
    }
}
