using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Branches.Entities;

namespace Garage.Application.Branches.Commands.Delete;

public class DeleteBranchHandler : BaseCommandHandler<DeleteBranchCommand, bool>
{
    private readonly IRepository<Branch> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBranchHandler(IRepository<Branch> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<bool>> Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        // Fetch entity
        var entity = await _repository.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);
        
        // Soft delete entity
        await _repository.SoftDeleteAsync(entity, ct: ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Ok(true);
    }
}

