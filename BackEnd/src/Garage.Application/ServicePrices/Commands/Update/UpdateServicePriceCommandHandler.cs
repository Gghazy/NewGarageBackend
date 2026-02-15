using Garage.Application.Abstractions;
using Garage.Application.ServicePrices.Commands.Update;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.ServicePrices.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed class UpdateServicePriceCommandHandler(
    IRepository<ServicePrice> repo,
    IUnitOfWork uow
) : IRequestHandler<UpdateServicePriceCommand, Guid>
{
    public async Task<Guid> Handle(UpdateServicePriceCommand command, CancellationToken cancellationToken)
    {
        var r = command.Request;

        var entity = await repo.Query()
            .FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);

        if (entity is null)
            throw new DomainException("Service price not found.");

        var hasOverlap = await repo.Query()
            .AsNoTracking()
            .Where(x => x.ServiceId == entity.ServiceId && x.MarkId == entity.MarkId && x.Id != entity.Id)
            .AnyAsync(x => x.FromYear <= r.ToYear && x.ToYear >= r.FromYear, cancellationToken);

        if (hasOverlap)
            throw new DomainException("Year range overlaps with an existing price for the same service and mark.");

        entity.Update(r.FromYear, r.ToYear, r.Price);

        await uow.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}

