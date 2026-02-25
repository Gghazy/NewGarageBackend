using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.AddPayment;

public sealed record AddPaymentCommand(Guid ExaminationId, AddPaymentRequest Request)
    : IRequest<Result<Guid>>;
