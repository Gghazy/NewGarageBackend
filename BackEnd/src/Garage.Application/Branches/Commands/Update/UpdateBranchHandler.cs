using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Branches.Entities;

namespace Garage.Application.Branches.Commands.Update;

public class UpdateBranchHandler : BaseCommandHandler<UpdateBranchCommand, bool>
{
    private readonly IRepository<Branch> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBranchHandler(IRepository<Branch> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<bool>> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        // Fetch entity
        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        // Update entity with domain logic
        entity.Update(request.Request.NameAr, request.Request.NameEn);
        
        if (request.Request.IsActive is not null)
        {
            if (request.Request.IsActive.Value)
                entity.Activate();
            else
                entity.Deactivate();
        }

        // Persist changes
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Ok(true);
    }
}

