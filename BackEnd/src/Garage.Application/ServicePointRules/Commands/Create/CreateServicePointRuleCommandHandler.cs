using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePointRules.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServicePointRules.Commands.Create;

public sealed class CreateServicePointRuleCommandHandler : BaseCommandHandler<CreateServicePointRuleCommand, Guid>
{
    private readonly IRepository<ServicePointRule> _repo;
    private readonly IUnitOfWork _uow;

    public CreateServicePointRuleCommandHandler(IRepository<ServicePointRule> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(CreateServicePointRuleCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var hasOverlap = await _repo.Query()
            .AsNoTracking()
            .AnyAsync(x => x.FromAmount < r.ToAmount && x.ToAmount > r.FromAmount, cancellationToken);

        if (hasOverlap)
            return Fail("Amount range overlaps with an existing point rule.");

        var entity = new ServicePointRule(r.FromAmount, r.ToAmount, r.Points, r.IsActive);

        await _repo.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Ok(entity.Id);
    }
}
