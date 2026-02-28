using Garage.Contracts.MechParts;
using MediatR;

namespace Garage.Application.MechParts.Queries.GetAllList;

public record GetAllMechPartsQuery() : IRequest<List<MechPartResponse>>;
