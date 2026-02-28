using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Domain.ExaminationManagement.Examinations;

namespace Garage.Application.Examinations.Commands.Delete;

public class DeleteExaminationHandler(
    IRepository<Examination> repository,
    IUnitOfWork unitOfWork)
    : BaseCommandHandler<DeleteExaminationCommand, bool>
{
    public override async Task<Result<bool>> Handle(DeleteExaminationCommand request, CancellationToken ct)
    {
        var entity = await repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        entity.MarkDeleted();
        await repository.SoftDeleteAsync(entity, ct: ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Ok(true);
    }
}
