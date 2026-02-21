using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Clients.Entities;

namespace Garage.Application.Clients.Commands.Delete;

public sealed class DeleteClientHandler : BaseCommandHandler<DeleteClientCommand, bool>
{
    private readonly IRepository<Client> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identityService;

    public DeleteClientHandler(IRepository<Client> repo, IUnitOfWork uow, IIdentityService identityService)
    {
        _repo = repo;
        _uow = uow;
        _identityService = identityService;
    }

    public override async Task<Result<bool>> Handle(DeleteClientCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);
        await _identityService.LockUserAsync(entity.UserId, ct);

        return Ok(true);
    }
}
