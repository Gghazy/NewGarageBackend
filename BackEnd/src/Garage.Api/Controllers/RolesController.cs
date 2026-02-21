using Garage.Api.Controllers.Common;
using Garage.Application.Roles.Commands;
using Garage.Application.Roles.Queries.GetAllRoles;
using Garage.Application.Roles.Queries.GetRoleById;
using Garage.Contracts.Roles;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RolesController : ApiControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator, IStringLocalizer localizer)
            : base(localizer)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [HasPermission(Permission.Roles_Create)]
        public async Task<IActionResult> Create(UpsertRoleRequest req)
        {
            var result = await _mediator.Send(new UpsertRoleWithPermissionsCommand(req));
            return Success(result);
        }

        [HttpGet]
        [HasPermission(Permission.Roles_Read)]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Success(roles);
        }

        [HttpGet("{id:guid}")]
        [HasPermission(Permission.Roles_Read)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var role = await _mediator.Send(new GetRoleByIdQuery(id));
            return Success(role);
        }
    }
}
