using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.InvoiceManagement.Invoices;

namespace Garage.Application.Invoices.Commands.Delete;

public class DeleteInvoiceHandler(
    IRepository<Invoice> repository,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<DeleteInvoiceCommand, bool>
{
    public override async Task<Result<bool>> Handle(DeleteInvoiceCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await repository.SoftDeleteAsync(entity, ct: ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
