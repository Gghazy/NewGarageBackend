using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;

namespace Garage.Application.Employees.Commands.Delete;

public sealed class DeleteEmployeeHandler : BaseCommandHandler<DeleteEmployeeCommand, bool>
{
    private readonly IRepository<Employee> _repo;
    private readonly IUnitOfWork _uow;
    private readonly IIdentityService _identityService;
    private readonly UserManager<AppUser> _userManager;

    public DeleteEmployeeHandler(
        IRepository<Employee> repo,
        IUnitOfWork uow,
        IIdentityService identityService,
        UserManager<AppUser> userManager)
    {
        _repo = repo;
        _uow = uow;
        _identityService = identityService;
        _userManager = userManager;
    }

    public override async Task<Result<bool>> Handle(DeleteEmployeeCommand request, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(request.Id, ct);
        if (entity is null)
            return Fail(NotFoundError);

        var user = await _userManager.FindByIdAsync(entity.UserId.ToString());
        if (user is not null)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any(r => r is "Admin" or "Manager"))
                return Fail("This employee is protected and cannot be deleted");
        }

        await _repo.SoftDeleteAsync(entity, ct: ct);
        await _uow.SaveChangesAsync(ct);
        await _identityService.LockUserAsync(entity.UserId, ct);

        return Ok(true);
    }
}
