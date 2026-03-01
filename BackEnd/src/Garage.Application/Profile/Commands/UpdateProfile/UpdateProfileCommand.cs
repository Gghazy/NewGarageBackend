using Garage.Application.Common;
using Garage.Contracts.Profile;
using MediatR;

namespace Garage.Application.Profile.Commands.UpdateProfile;

public sealed record UpdateProfileCommand(UpdateProfileRequest Request) : IRequest<Result<UpdateProfileResponse>>;
