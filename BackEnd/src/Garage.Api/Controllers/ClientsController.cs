
using Garage.Application.Clients.Commands.Create;
using Garage.Application.Clients.Commands.Update;
using Garage.Application.Clients.Queries.GetAllClients;
using Garage.Application.Clients.Queries.GetAllClientsBySearch;
using Garage.Contracts.Clients;
using Garage.Contracts.Common;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Garage.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientsController(IMediator mediator, IStringLocalizer T) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await mediator.Send(new GetAllClientsQuery()));


    [HttpPost("pagination")]
    [HasPermission(Permission.Client_Read)]
    public async Task<IActionResult> GetAll(SearchCriteria search) => Ok(await mediator.Send(new GetAllClientsBySearchQuery(search)));

    [HttpPost]
    [HasPermission(Permission.Client_Create)]
    public async Task<IActionResult> Create(CreateClientRequest request)
    {
        var res = await mediator.Send(new CreateClientCommand(request));
        return Ok(res);
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Client_Update)]
    public async Task<IActionResult> Update(Guid id, CreateClientRequest request)
    {
        var res = await mediator.Send(new UpdateClientCommand(id, request));
        return Ok(res);
    }
}

