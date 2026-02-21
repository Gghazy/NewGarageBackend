using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Terms.Entity;

namespace Garage.Application.Terms.Commands.Delete;

public sealed class DeleteTermHandler : BaseCommandHandler<DeleteTermCommand, bool>
{
    private readonly IRepository<Term> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteTermHandler(IRepository<Term> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteTermCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
