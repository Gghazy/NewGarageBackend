using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePrices.Entities;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateServicePriceCommandHandler : BaseCommandHandler<UpdateServicePriceCommand, bool>
{
    private readonly IRepository<ServicePrice> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateServicePriceCommandHandler(
        IRepository<ServicePrice> repo,
        IUnitOfWork uow
    )
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(UpdateServicePriceCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var entity = await _repo.Query()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (entity is null)
            return Fail(NotFoundError);

        var hasOverlap = await _repo.Query()
            .AsNoTracking()
            .Where(x => x.ServiceId == entity.ServiceId && x.MarkId == entity.MarkId && x.Id != entity.Id)
            .AnyAsync(x => x.FromYear <= r.ToYear && x.ToYear >= r.FromYear, cancellationToken);

        if (hasOverlap)
            return Fail("Year range overlaps with an existing price for the same service and mark.");

        entity.Update(r.FromYear, r.ToYear, r.Price);

        await _uow.SaveChangesAsync(cancellationToken);

        return Ok(true);
    }
}

