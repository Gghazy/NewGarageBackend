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
    IApplicationDbContext _context,
    UserManager<AppUser> _userManager,
    IReadRepository<Employee> _empRepository,
    IUnitOfWork _unitOfWork
  ) : IRequestHandler<UpdateEmployeeCommand,Guid>
{
    public async Task<Guid> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var employee = await _empRepository.QueryTracking()
            .FirstOrDefaultAsync(x => x.Id == command.Id, ct);

        if (employee is null)
            throw new Exception("Employee not found");

        employee.update(command.Request.NameAr, command.Request.NameEn, command.Request.BranchId);

        var user = await _userManager.FindByIdAsync(employee.UserId.ToString());
        if (user is null)
            throw new Exception("User not found");

        if (!string.Equals(user.Email, command.Request.Email, StringComparison.OrdinalIgnoreCase))
        {
            user.Email = command.Request.Email;
            user.NormalizedEmail = command.Request.Email?.ToUpperInvariant();
            user.EmailConfirmed = true; 
        }

        user.PhoneNumber = command.Request.PhoneNumber;

        var updateRes = await _userManager.UpdateAsync(user);
        if (!updateRes.Succeeded)
            throw new Exception(string.Join(", ", updateRes.Errors.Select(e => e.Description)));

        if (command.Request.RoleId!=null)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == command.Request.RoleId, ct);
            if (!roleExists)
                throw new Exception("Role not found");

            var oldRoles = _context.UserRoles.Where(x => x.UserId == user.Id);
            _context.UserRoles.RemoveRange(oldRoles);

            _context.UserRoles.Add(new IdentityUserRole<Guid>
            {
                UserId = user.Id,
                RoleId = command.Request.RoleId
            });
        }

        await _unitOfWork.SaveChangesAsync(ct);

        return employee.Id;
    }
}
