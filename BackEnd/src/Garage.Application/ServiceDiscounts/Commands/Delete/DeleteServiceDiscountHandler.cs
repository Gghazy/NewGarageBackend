using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.ServiceDiscounts.Entities;

namespace Garage.Application.ServiceDiscounts.Commands.Delete;

public sealed class DeleteServiceDiscountHandler : BaseCommandHandler<DeleteServiceDiscountCommand, bool>
{
    private readonly IRepository<ServiceDiscount> _repo;
    private readonly IUnitOfWork _uow;

    public DeleteServiceDiscountHandler(IRepository<ServiceDiscount> repo, IUnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public override async Task<Result<bool>> Handle(DeleteServiceDiscountCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);

        return Ok(true);
    }
}
