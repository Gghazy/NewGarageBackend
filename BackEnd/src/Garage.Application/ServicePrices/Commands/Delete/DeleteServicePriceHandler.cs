using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServicePrices.Entities;

namespace Garage.Application.ServicePrices.Commands.Delete;

public sealed class DeleteServicePriceHandler : BaseCommandHandler<DeleteServicePriceCommand, bool>
{
    private readonly IRepository<ServicePrice> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteServicePriceHandler(IRepository<ServicePrice> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteServicePriceCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
