using Garage.Contracts.CarMarks;
using MediatR;

namespace Garage.Application.CarMarks.Queries.GetAll;

public sealed record GetAllCarMarksQuery() : IRequest<List<CarMarkDto>>;
