using Domain.ExaminationManagement.Examinations;
using Garage.Api.Controllers.Common;
using Garage.Application.Bookings.Commands.ChangeStatus;
using Garage.Application.Bookings.Commands.Convert;
using Garage.Application.Bookings.Commands.Create;
using Garage.Application.Bookings.Commands.Delete;
using Garage.Application.Bookings.Commands.Update;
using Garage.Application.Bookings.Queries.GetByExaminationId;
using Garage.Application.Bookings.Queries.GetById;
using Garage.Application.Bookings.Queries.GetHistory;
using Garage.Application.Bookings.Queries.Search;
using Garage.Contracts.Bookings;
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
public class BookingsController : ApiControllerBase
{
    private readonly IMediator _mediator;

    public BookingsController(IMediator mediator, IStringLocalizer localizer)
        : base(localizer)
    {
        _mediator = mediator;
    }

    [HttpPost("pagination")]
    [HasPermission(Permission.Booking_Read)]
    public async Task<IActionResult> Search(SearchCriteria search)
    {
        var result = await _mediator.Send(new SearchBookingsQuery(search));
        return Success(result);
    }

    [HttpGet("{id:Guid}")]
    [HasPermission(Permission.Booking_Read)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetBookingByIdQuery(id));
        if (result is null) return NotFound();
        return Success(result);
    }

    [HttpPost]
    [HasPermission(Permission.Booking_Create)]
    public async Task<IActionResult> Create(CreateBookingRequest request)
    {
        var result = await _mediator.Send(new CreateBookingCommand(request));
        return HandleResult(result, "Booking.Created");
    }

    [HttpPut("{id:Guid}")]
    [HasPermission(Permission.Booking_Update)]
    public async Task<IActionResult> Update(Guid id, UpdateBookingRequest request)
    {
        var result = await _mediator.Send(new UpdateBookingCommand(id, request));
        return HandleResult(result, "Booking.Updated");
    }

    [HttpDelete("{id:Guid}")]
    [HasPermission(Permission.Booking_Delete)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteBookingCommand(id));
        return HandleResult(result, "Booking.Deleted");
    }

    [HttpPost("{id:Guid}/confirm")]
    [HasPermission(Permission.Booking_Update)]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await _mediator.Send(new ChangeBookingStatusCommand(id, "confirm"));
        return HandleResult(result, "Booking.Confirmed");
    }

    [HttpPost("{id:Guid}/cancel")]
    [HasPermission(Permission.Booking_Update)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _mediator.Send(new ChangeBookingStatusCommand(id, "cancel"));
        return HandleResult(result, "Booking.Cancelled");
    }

    [HttpPost("{id:Guid}/convert")]
    [HasPermission(Permission.Booking_Convert)]
    public async Task<IActionResult> Convert(Guid id)
    {
        var result = await _mediator.Send(new ConvertBookingCommand(id));
        return HandleResult(result, "Booking.Converted");
    }

    [HttpGet("{id:Guid}/history")]
    [HasAnyPermission(Permission.Booking_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetHistory(Guid id)
    {
        var result = await _mediator.Send(new GetBookingHistoryQuery(id));
        return Success(result);
    }

    [HttpGet("by-examination/{examinationId:Guid}")]
    [HasAnyPermission(Permission.Booking_Read, Permission.Examination_Read)]
    public async Task<IActionResult> GetByExaminationId(Guid examinationId)
    {
        var result = await _mediator.Send(new GetBookingByExaminationIdQuery(examinationId));
        if (result is null) return NotFound();
        return Success(result);
    }
}
