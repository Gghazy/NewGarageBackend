using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Employees.Commands.Update;

public sealed class UpdateEmployeeCommandHandler : BaseCommandHandler<UpdateEmployeeCommand, bool>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IReadRepository<Employee> _empRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEmployeeCommandHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IReadRepository<Employee> empRepository,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _empRepository = empRepository;
        _unitOfWork = unitOfWork;
    }

    public override async Task<Result<bool>> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    public override async Task<Result<bool>> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await _empRepository.QueryTracking()
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (employee is null)
            return Fail(NotFoundError);

        employee.update(command.Request.NameAr, command.Request.NameEn, command.Request.BranchId);
        await _unitOfWork.SaveChangesAsync(ct);

        var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
        if (user is null)
            return Fail("User not found");

        bool needUpdate = false;

        if (!string.Equals(user.Email, command.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = command.Request.Email;
            user.NormalizedEmail = command.Request.Email?.ToUpperInvariant();
            user.EmailConfirmed = true;
            needUpdate = true;
        }

        if (user.PhoneNumber != command.Request.PhoneNumber)
        {
            user.PhoneNumber = command.Request.PhoneNumber;
            needUpdate = true;
        }

        if (needUpdate)
        {
            var updateRes = await _userManager.UpdateAsync(user);
            if (!updateRes.Succeeded)
                return Fail($"Failed to update user: {string.Join(", ", updateRes.Errors.Select(e => e.Description))}");
        }

        if (command.Request.RoleId != null)
        {
            var role = await _roleManager.FindByIdAsync(command.Request.RoleId.ToString());
            if (role is null)
                return Fail("Role not found");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                    return Fail($"Failed to remove roles: {string.Join(", ", removeRes.Errors.Select(e => e.Description))}");
            }

            var addRes = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!addRes.Succeeded)
                return Fail($"Failed to assign role: {string.Join(", ", addRes.Errors.Select(e => e.Description))}");
        }

        return Ok(true);
    }
}
