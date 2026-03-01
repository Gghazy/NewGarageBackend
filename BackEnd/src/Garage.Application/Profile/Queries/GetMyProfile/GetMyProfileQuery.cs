using Garage.Contracts.Profile;
using MediatR;

namespace Garage.Application.Profile.Queries.GetMyProfile;

public sealed record GetMyProfileQuery() : IRequest<ProfileDto?>;
