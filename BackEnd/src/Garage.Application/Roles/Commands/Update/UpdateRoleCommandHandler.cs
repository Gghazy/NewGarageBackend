using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Garage.Application.Roles.Commands.Update;

public sealed class UpdateRoleCommandHandler(RoleManager<AppRole> _roleManager)
    : BaseCommandHandler<UpdateRoleCommand, Guid>
{
    public override async Task<Result<Guid>> Handle(UpdateRoleCommand command, CancellationToken ct)
    {
        var role = await _roleManager.FindByIdAsync(command.Id.ToString());
        if (role is null)
            return Fail("Role not found");

        if (role.Name is "Admin" or "Manager")
            return Fail("This role is protected and cannot be modified");

        var newName = command.Request.RoleName.Trim();

        if (newName.Equals("Admin", StringComparison.OrdinalIgnoreCase) ||
            newName.Equals("Manager", StringComparison.OrdinalIgnoreCase))
            return Fail("This role name is reserved");

        if (!string.Equals(role.Name, newName, StringComparison.OrdinalIgnoreCase))
        {
            var duplicate = await _roleManager.FindByNameAsync(newName);
            if (duplicate is not null)
                return Fail("Role name already exists");

            role.Name = newName;
            role.NormalizedName = newName.ToUpperInvariant();
            var updateResult = await _roleManager.UpdateAsync(role);
            if (!updateResult.Succeeded)
                return Fail(string.Join(", ", updateResult.Errors.Select(e => e.Description)));
        }

        var incoming = (command.Request.Permissions ?? new List<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existingClaims = await _roleManager.GetClaimsAsync(role);
        var existingPermissions = existingClaims
            .Where(c => c.Type == Permission.ClaimType)
            .Select(c => c.Value)
            .ToList();

        // Remove permissions that are no longer in the incoming list
        foreach (var claim in existingClaims.Where(c => c.Type == Permission.ClaimType))
        {
            if (!incoming.Any(p => string.Equals(p, claim.Value, StringComparison.OrdinalIgnoreCase)))
            {
                var removeResult = await _roleManager.RemoveClaimAsync(role, claim);
                if (!removeResult.Succeeded)
                    return Fail(string.Join(", ", removeResult.Errors.Select(e => e.Description)));
            }
        }

        // Add new permissions
        foreach (var p in incoming)
        {
            if (!Permission.All.Contains(p, StringComparer.OrdinalIgnoreCase))
                return Fail($"Invalid permission: {p}");

            if (!existingPermissions.Any(ep => string.Equals(ep, p, StringComparison.OrdinalIgnoreCase)))
            {
                var addResult = await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimType, p));
                if (!addResult.Succeeded)
                    return Fail(string.Join(", ", addResult.Errors.Select(e => e.Description)));
            }
        }

        return Ok(role.Id);
    }
}
