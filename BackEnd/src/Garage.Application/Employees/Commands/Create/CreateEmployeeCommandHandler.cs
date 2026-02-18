using Garage.Application.Abstractions;
using Garage.Domain.Employees.Entities;
using Garage.Domain.MechIssues.Entities;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Employees.Commands.Create;

public class CreateEmployeeCommandHandler(IRepository<Employee> repo, IUnitOfWork _ofWork, UserManager<AppUser> _userManager, RoleManager<AppRole> _roleManager) : IRequestHandler<CreateEmployeeCommand, Guid>
{


    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = command.Request.PhoneNumber,
            Email = command.Request.Email,
            PhoneNumber = command.Request.PhoneNumber,
            EmailConfirmed = true
        };

        var createUser = await _userManager.CreateAsync(user, "123456");

        if (!createUser.Succeeded)
            throw new Exception(string.Join(",", createUser.Errors.Select(e => e.Description)));

        var role = await _roleManager.FindByIdAsync(command.Request.RoleId.ToString());
        if (role is null)
            throw new Exception("Role not found");


        var addToRole = await _userManager.AddToRoleAsync(user, role.Name!);
        if (!addToRole.Succeeded)
            throw new Exception(string.Join(",", addToRole.Errors.Select(e => e.Description)));



        var employee = new Employee(
            user.Id,
            command.Request.NameAr,
            command.Request.NameEn,
            command.Request.BranchId
        );

        await repo.AddAsync(employee);
        await _ofWork.SaveChangesAsync(ct);

        return employee.Id;
    }
}
