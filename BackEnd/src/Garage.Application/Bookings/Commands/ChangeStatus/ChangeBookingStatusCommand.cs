using Garage.Application.Common;
using MediatR;

namespace Garage.Application.Bookings.Commands.ChangeStatus;

public record ChangeBookingStatusCommand(Guid Id, string Action) : IRequest<Result<bool>>;
