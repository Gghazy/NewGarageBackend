using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServiceDiscounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServiceDiscounts.Commands.Update;

public sealed class UpdateServiceDiscountCommandHandler : BaseCommandHandler<UpdateServiceDiscountCommand, bool>
{
    private readonly IRepository<ServiceDiscount> _repo;
    private readonly IUnitOfWork _uow;

    public UpdateServiceDiscountCommandHandler(IRepository<ServiceDiscount> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(UpdateServiceDiscountCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var entity = await _repo.Query()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (entity is null)
            return Fail(NotFoundError);

        var hasOverlap = await _repo.Query()
            .AsNoTracking()
            .Where(x => x.ServiceId == entity.ServiceId && x.Id != entity.Id)
            .AnyAsync(x => x.StartDate <= r.EndDate && x.EndDate >= r.StartDate, cancellationToken);

        if (hasOverlap)
            return Fail("Date range overlaps with an existing discount for the same service.");

        entity.Update(r.DiscountPercent, r.StartDate, r.EndDate, r.IsActive);

        await _uow.SaveChangesAsync(cancellationToken);

        return Ok(true);
    }
}
