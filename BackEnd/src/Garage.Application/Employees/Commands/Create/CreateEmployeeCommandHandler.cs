using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;

namespace Garage.Application.Employees.Commands.Create;

public class CreateEmployeeCommandHandler : BaseCommandHandler<CreateEmployeeCommand, Guid>
{
    private readonly IRepository<Employee> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public CreateEmployeeCommandHandler(
        IRepository<Employee> repository,
        IUnitOfWork unitOfWork,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public override async Task<Result<Guid>> Handle(CreateEmployeeCommand command, CancellationToken ct)
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
            return Fail($"Failed to create user: {string.Join(", ", createUser.Errors.Select(e => e.Description))}");

        var role = await _roleManager.FindByIdAsync(command.Request.RoleId.ToString());
        if (role is null)
            return Fail("Role not found");

        var addToRole = await _userManager.AddToRoleAsync(user, role.Name!);
        if (!addToRole.Succeeded)
            return Fail($"Failed to assign role: {string.Join(", ", addToRole.Errors.Select(e => e.Description))}");

        var employee = new Employee(
            user.Id,
            command.Request.NameAr,
            command.Request.NameEn,
            command.Request.BranchId
        );

        await _repository.AddAsync(employee, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Ok(employee.Id);
    }
}
