using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Garage.Application.Roles.Commands.Create;

public sealed class CreateRoleCommandHandler(RoleManager<AppRole> _roleManager)
    : BaseCommandHandler<CreateRoleCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(CreateRoleCommand command, CancellationToken ct)
    {
        var roleName = command.Request.RoleName.Trim();

        if (roleName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            roleName.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            return Fail("This role name is reserved");

        var existing = await _roleManager.FindByNameAsync(roleName);
        if (existing is not null)
            return Fail("Role name already exists");

        var role = new AppRole
        {
            Id = Guid.NewGuid(),
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        };

        var createResult = await _roleManager.CreateAsync(role);
        if (!createResult.Succeeded)
            return Fail(string.Join(", ", createResult.Errors.Select(e => e.Description)));

        var permissions = (command.Request.Permissions ?? new List<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var p in permissions)
        {
            if (!Permission.All.Contains(p, StringComparer.OrdinalIgnoreCase))
                return Fail($"Invalid permission: {p}");

            var addResult = await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimType, p));
            if (!addResult.Succeeded)
                return Fail(string.Join(", ", addResult.Errors.Select(e => e.Description)));
        }

        return Ok(role.Id);
    }
}
