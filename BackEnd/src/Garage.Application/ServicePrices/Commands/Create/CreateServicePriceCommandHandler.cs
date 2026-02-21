using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePrices.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.ServicePrices.Commands.Create
{
    public sealed class CreateServicePriceCommandHandler : BaseCommandHandler<CreateServicePriceCommand, Guid>
    {
        private readonly IRepository<ServicePrice> _repo;
        private readonly IUnitOfWork _uow;

        public CreateServicePriceCommandHandler(IRepository<ServicePrice> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public override async Task<Result<Guid>> Handle(CreateServicePriceCommand command, CancellationToken cancellationToken)
        {
            var r = command.Request;

            var hasOverlap = await _repo.Query()
                                       .AsNoTracking()
                                       .Where(x => x.ServiceId == r.ServiceId && x.MarkId == r.MarkId)
                                       .AnyAsync(x => x.FromYear <= r.ToYear && x.ToYear >= r.FromYear, cancellationToken);

            if (hasOverlap)
                return Fail("Year range overlaps with an existing price for the same service and mark.");

            var entity = new ServicePrice(
                r.ServiceId,
                r.MarkId,
                r.FromYear,
                r.ToYear,
                r.Price);

            await _repo.AddAsync(entity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Ok(entity.Id);
        }
    }
}
