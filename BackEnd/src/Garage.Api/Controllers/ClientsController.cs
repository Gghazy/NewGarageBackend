using Garage.Api.Controllers.Common;
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
public class ClientsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public ClientsController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Gets all clients
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var clients = await _mediator.Send(new GetAllClientsQuery());
        return Success(clients);
    }

    /// <summary>
    /// Searches clients with pagination
    /// </summary>
    [HttpPost("pagination")]
    [HasPermission(Permission.Client_Read)]
    public async Task<IActionResult> Search(SearchCriteria search)
    {
        var result = await _mediator.Send(new GetAllClientsBySearchQuery(search));
        return Success(result);
    }

    /// <summary>
    /// Creates a new client
    /// </summary>
    [HttpPost]
    [HasPermission(Permission.Client_Create)]
    public async Task<IActionResult> Create(CreateClientRequest request)
    {
        var result = await _mediator.Send(new CreateClientCommand(request));
        return HandleResult(result, "Client.Created");
    }

    /// <summary>
    /// Updates an existing client
    /// </summary>
    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Client_Update)]
    public async Task<IActionResult> Update(Guid id, CreateClientRequest request)
    {
        var result = await _mediator.Send(new UpdateClientCommand(id, request));
        return HandleResult(result, "Client.Updated");
    }
}
