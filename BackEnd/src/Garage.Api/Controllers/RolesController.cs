using Garage.Application.Common;
using Garage.Application.Roles.Commands;
using Garage.Application.Roles.Queries.GetAllRoles;
using Garage.Application.Roles.Queries.GetRoleById;
using Garage.Application.Terms.Commands.Create;
using Garage.Contracts.Roles;
using Garage.Contracts.Terms;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Garage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController(IMediator _mediator) : ControllerBase
    {
        [HttpPost]
        [HasPermission(Permission.Roles_Create)]
        public async Task<Guid> Create(UpsertRoleRequest req  )
         => await _mediator.Send(new UpsertRoleWithPermissionsCommand(req));


        [HttpGet]
        [HasPermission(Permission.Roles_Read)]
        public async Task<List<RoleDto>> GetAll()
                     => await _mediator.Send(new GetAllRolesQuery());


        [HttpGet("{id:guid}")]
        [HasPermission(Permission.Roles_Read)]
        public async Task<RoleDetailsDto> GetById(Guid id)
            => await _mediator.Send(new GetRoleByIdQuery(id));


    }
}
