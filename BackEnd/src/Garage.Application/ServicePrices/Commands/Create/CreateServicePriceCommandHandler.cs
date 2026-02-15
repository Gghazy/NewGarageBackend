using Garage.Application.Abstractions;
using Garage.Domain.Common.Exceptions;
using Garage.Domain.SensorIssues.Entities;
using Garage.Domain.ServicePrices.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.ServicePrices.Commands.Create
{
    public sealed class CreateServicePriceCommandHandler(IRepository<ServicePrice> repo, IUnitOfWork uow) : IRequestHandler<CreateServicePriceCommand, Guid>
    {
        public async Task<Guid> Handle(CreateServicePriceCommand command, CancellationToken cancellationToken)
        {
            var r=command.Request;

            var hasOverlap = await repo.Query()
                                       .AsNoTracking()
                                       .Where(x => x.ServiceId == r.ServiceId && x.MarkId == r.MarkId)
                                       .AnyAsync(x => x.FromYear <= r.ToYear && x.ToYear >= r.FromYear, cancellationToken);

            if (hasOverlap)
                throw new DomainException("Year range overlaps with an existing price for the same service and mark.");


            var entity = new ServicePrice(
                r.ServiceId,
                r.MarkId,
                r.FromYear,
                r.ToYear,
                r.Price);

            await repo.AddAsync(entity, cancellationToken);
            await uow.SaveChangesAsync(cancellationToken);

            return entity.Id;
        }
    }
}
