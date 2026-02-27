using Garage.Contracts.Examinations;
using MediatR;

namespace Garage.Application.Examinations.Queries.GetWorkflow;

public sealed record GetExaminationWorkflowQuery(Guid Id) : IRequest<ExaminationWorkflowDto?>;
