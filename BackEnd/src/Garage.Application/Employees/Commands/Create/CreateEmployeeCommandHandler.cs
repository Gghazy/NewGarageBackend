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

namespace Garage.Application.Employees.Commands.Create;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
{
    private readonly IUnitOfWork _ofWork;
    private readonly IApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CreateEmployeeCommandHandler(IUnitOfWork ofWork = null, IApplicationDbContext context = null, UserManager<AppUser> userManager = null)
    {
        _ofWork = ofWork;
        _context = context;
        _userManager = userManager;
    }

    public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken ct)
    {
        // 1️⃣ إنشاء اليوزر
        var user = new AppUser
        {
            Id = Guid.NewGuid(),
            UserName = command.Request.PhoneNumber,
            Email = command.Request.PhoneNumber,
            PhoneNumber = command.Request.PhoneNumber,
            EmailConfirmed = true
        };

        var createUser = await _userManager.CreateAsync(user, "123456");

        if (!createUser.Succeeded)
            throw new Exception(string.Join(",", createUser.Errors.Select(e => e.Description)));

        var roleExists = await _context.Roles
           .AnyAsync(r => r.Id == command.Request.RoleId, ct);

        if (!roleExists)
            throw new Exception("Role not found");

        _context.UserRoles.Add(new IdentityUserRole<Guid>
        {
            UserId = user.Id,
            RoleId = command.Request.RoleId
        });



        var employee = new Employee(
            user.Id,
            command.Request.NameAr,
            command.Request.NameEn,
            command.Request.BranchId
        );

        _context.Employees.Add(employee);
        await _ofWork.SaveChangesAsync(ct);

        return employee.Id;
    }
}
