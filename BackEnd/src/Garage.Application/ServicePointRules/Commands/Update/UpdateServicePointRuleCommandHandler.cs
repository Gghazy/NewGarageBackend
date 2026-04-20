using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePointRules.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServicePointRules.Commands.Update;

public sealed class UpdateServicePointRuleCommandHandler : BaseCommandHandler<UpdateServicePointRuleCommand, bool>
{
    private readonly IRepository<ServicePointRule> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateServicePointRuleCommandHandler(IRepository<ServicePointRule> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(UpdateServicePointRuleCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var entity = await _repo.Query()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (entity is null)
            return Fail(NotFoundError);

        var hasOverlap = await _repo.Query()
            .AsNoTracking()
            .Where(x => x.Id != entity.Id)
            .AnyAsync(x => x.FromAmount < r.ToAmount && x.ToAmount > r.FromAmount, cancellationToken);

        if (hasOverlap)
            return Fail("Amount range overlaps with an existing point rule.");

        entity.Update(r.FromAmount, r.ToAmount, r.Points, r.IsActive);
        await _uow.SaveChangesAsync(cancellationToken);

        return Ok(true);
    }
}
