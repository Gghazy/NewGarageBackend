using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServiceDiscounts.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServiceDiscounts.Commands.Create;

public sealed class CreateServiceDiscountCommandHandler : BaseCommandHandler<CreateServiceDiscountCommand, Guid>
{
    private readonly IRepository<ServiceDiscount> _repo;
    private readonly IUnitOfWork _uow;

    public CreateServiceDiscountCommandHandler(IRepository<ServiceDiscount> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<Guid>> Handle(CreateServiceDiscountCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var hasOverlap = await _repo.Query()
            .AsNoTracking()
            .Where(x => x.ServiceId == r.ServiceId)
            .AnyAsync(x => x.StartDate <= r.EndDate && x.EndDate >= r.StartDate, cancellationToken);

        if (hasOverlap)
            return Fail("Date range overlaps with an existing discount for the same service.");

        var entity = new ServiceDiscount(
            r.ServiceId,
            r.DiscountPercent,
            r.StartDate,
            r.EndDate,
            r.IsActive);

        await _repo.AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Ok(entity.Id);
    }
}
