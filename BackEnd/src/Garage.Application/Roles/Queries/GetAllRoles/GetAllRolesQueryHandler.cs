using Garage.Contracts.Roles;
using Garage.Infrastructure.Auth.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Roles.Queries.GetAllRoles
{
    public sealed class GetAllRolesQueryHandler(RoleManager<AppRole> _roleManager) : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken ct)
        {
            var roles = await _roleManager.Roles
             .AsNoTracking()
             .Where(x=>x.Name!="Admin")
             .OrderBy(r => r.Name)             
             .Select(r => new RoleDto(r.Id, r.Name!)) 
             .ToListAsync(ct);

            return roles;
        }
    }
}
