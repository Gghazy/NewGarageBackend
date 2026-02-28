using Garage.Contracts.CarMarks;
using MediatR;

namespace Garage.Application.CarMarks.Queries.GetByManufacturer;

public sealed record GetCarMarksByManufacturerQuery(Guid ManufacturerId) : IRequest<List<CarMarkDto>>;
