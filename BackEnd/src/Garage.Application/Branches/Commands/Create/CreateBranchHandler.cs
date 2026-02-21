using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Branches.Entities;

namespace Garage.Application.Branches.Commands.Create;

public class CreateBranchHandler : BaseCommandHandler<CreateBranchCommand, Guid>
{
    private readonly IRepository<Branch> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBranchHandler(IRepository<Branch> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<Guid>> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        var req = request.Request;
        
        // Create entity with business logic
        var entity = new Branch(req.NameAr, req.NameEn, req.IsActive);
        
        // Persist to database
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        
        return Ok(entity.Id);
    }
}

