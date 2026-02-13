using Garage.Application.Abstractions;
using Garage.Domain.Services.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Services.Commands.UpsertServicePrice
{
    public sealed class UpsertServicePriceHandler( IRepository<Service> repo, IUnitOfWork uow) : IRequestHandler<UpsertServicePriceCommand>
    {
        public async Task Handle(UpsertServicePriceCommand request, CancellationToken ct)
        {


            var service = await repo.Query()
                .Include(x => x.Prices)
                .FirstOrDefaultAsync(x => x.Id == request.ServiceId, ct)
                ?? throw new KeyNotFoundException("Service not found");

            var r = request.Request;

            service.UpsertPrice(r.MarkId, r.FromYear, r.ToYear, r.Price);

            await uow.SaveChangesAsync(ct);
        }
    }
}
