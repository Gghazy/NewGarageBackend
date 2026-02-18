using Garage.Application.Abstractions;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Employees.Commands.Update;

public sealed class UpdateEmployeeCommandHandler(
    UserManager<AppUser> _userManager,
    RoleManager<AppRole> _roleManager,
    IReadRepository<Employee> _empRepository,
    IUnitOfWork _unitOfWork
  ) : IRequestHandler<UpdateEmployeeCommand,Guid>
{
    public async Task<Guid> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        // =========================
        // 1) Update Employee (Domain)
        // =========================
        var employee = await _empRepository.QueryTracking()
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (employee is null)
            throw new Exception("Employee not found");

        employee.update(command.Request.NameAr, command.Request.NameEn, command.Request.BranchId);

        await _unitOfWork.SaveChangesAsync(ct); 

        var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
        if (user is null)
            throw new Exception("User not found");

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
                throw new Exception(string.Join(", ", updateRes.Errors.Select(e => e.Description)));
        }


        if (command.Request.RoleId != null)
        {
            var role = await _roleManager.FindByIdAsync(command.Request.RoleId.ToString());
            if (role is null)
                throw new Exception("Role not found");

            var currentRoles = await _userManager.GetRolesAsync(user);

            if (currentRoles.Any())
            {
                var removeRes = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!removeRes.Succeeded)
                    throw new Exception(string.Join(", ", removeRes.Errors.Select(e => e.Description)));
            }

            var addRes = await _userManager.AddToRoleAsync(user, role.Name!);
            if (!addRes.Succeeded)
                throw new Exception(string.Join(", ", addRes.Errors.Select(e => e.Description)));
        }

        return employee.Id;
    }

}
