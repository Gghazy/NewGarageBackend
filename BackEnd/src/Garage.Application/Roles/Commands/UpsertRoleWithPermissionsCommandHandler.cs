using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Roles.Commands;

public sealed class UpsertRoleWithPermissionsCommandHandler
 : IRequestHandler<UpsertRoleWithPermissionsCommand, Guid>
{
    private readonly RoleManager<AppRole> _roleManager;

    public UpsertRoleWithPermissionsCommandHandler(RoleManager<AppRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public async Task<Guid> Handle(UpsertRoleWithPermissionsCommand command, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(command.Request.RoleName))
            throw new Exception("RoleName is required");

        var roleName = command.Request.RoleName.Trim();

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role is null)
        {
            role = new AppRole
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            };

            var createRole = await _roleManager.CreateAsync(role);
            if (!createRole.Succeeded)
                throw new Exception(string.Join(", ", createRole.Errors.Select(e => e.Description)));
        }

        var incoming = (command.Request.Permissions ?? new List<string>())
            .Where(p => !string.IsNullOrWhiteSpace(p))
            .Select(p => p.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var existingClaims = await _roleManager.GetClaimsAsync(role);

        foreach (var p in incoming)
        {

            if (!Permission.All.Contains(p, StringComparer.OrdinalIgnoreCase))
                throw new Exception($"Invalid permission: {p}");

            var exists = existingClaims.Any(c =>
                c.Type == Permission.ClaimType &&
                string.Equals(c.Value, p, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                var add = await _roleManager.AddClaimAsync(role, new Claim(Permission.ClaimType, p));
                if (!add.Succeeded)
                    throw new Exception(string.Join(", ", add.Errors.Select(e => e.Description)));
            }
        }

        return role.Id;
    }
}
