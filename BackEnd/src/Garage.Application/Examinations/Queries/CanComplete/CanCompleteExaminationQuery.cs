using MediatR;

namespace Garage.Application.Examinations.Queries.CanComplete;

public sealed record CanCompleteExaminationQuery(Guid Id) : IRequest<bool>;
