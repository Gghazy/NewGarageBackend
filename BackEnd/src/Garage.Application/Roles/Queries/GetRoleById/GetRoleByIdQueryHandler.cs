using Garage.Contracts.Roles;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Application.Roles.Queries.GetRoleById
{
    public sealed class GetRoleByIdQueryHandler(RoleManager<AppRole> _roleManager) : IRequestHandler<GetRoleByIdQuery, RoleDetailsDto>
    {


        public async Task<RoleDetailsDto> Handle(GetRoleByIdQuery request, CancellationToken ct)
        {
            var role = await _roleManager.FindByIdAsync(request.Id.ToString());



            var claims = await _roleManager.GetClaimsAsync(role);

            var permissions = claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .OrderBy(x => x)
                .ToList();

            return new RoleDetailsDto(
                role.Id,
                role.Name!,
                permissions
            );
        }
    }
}
