using Garage.Application.Common;
using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Commands.RefundPayment;

public sealed record RefundPaymentCommand(Guid ExaminationId, AddPaymentRequest Request)
    : IRequest<Result<Guid>>;
