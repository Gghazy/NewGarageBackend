using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePointRules.Entities;

namespace Garage.Application.ServicePointRules.Commands.Delete;

public sealed class DeleteServicePointRuleHandler : BaseCommandHandler<DeleteServicePointRuleCommand, bool>
{
    private readonly IRepository<ServicePointRule> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteServicePointRuleHandler(IRepository<ServicePointRule> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteServicePointRuleCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
