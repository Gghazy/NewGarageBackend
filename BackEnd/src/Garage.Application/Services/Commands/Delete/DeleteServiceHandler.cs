using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Services.Entities;

namespace Garage.Application.Services.Commands.Delete;

public sealed class DeleteServiceHandler : BaseCommandHandler<DeleteServiceCommand, bool>
{
    private readonly IRepository<Service> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteServiceHandler(IRepository<Service> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteServiceCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
