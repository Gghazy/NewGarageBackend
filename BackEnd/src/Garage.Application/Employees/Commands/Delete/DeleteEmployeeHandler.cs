using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Employees.Entities;

namespace Garage.Application.Employees.Commands.Delete;

public sealed class DeleteEmployeeHandler : BaseCommandHandler<DeleteEmployeeCommand, bool>
{
    private readonly IRepository<Employee> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identityService;

    public DeleteEmployeeHandler(IRepository<Employee> repo, IUnitOfWork uow, IIdentityService identityService)
    {
        _repo = repo;
        _uow = uow;
        _identityService = identityService;
    }

    public override async Task<Result<bool>> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);
        await _identityService.LockUserAsync(entity.UserId, ct);

        return Ok(true);
    }
}
