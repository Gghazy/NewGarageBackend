using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.MechParts.Entities;

namespace Garage.Application.MechParts.Commands.Delete;

public sealed class DeleteMechPartHandler : BaseCommandHandler<DeleteMechPartCommand, bool>
{
    private readonly IRepository<MechPart> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteMechPartHandler(IRepository<MechPart> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteMechPartCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
