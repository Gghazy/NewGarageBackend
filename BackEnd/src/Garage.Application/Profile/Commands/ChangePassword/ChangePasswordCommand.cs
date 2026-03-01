using Garage.Application.Common;
using Garage.Contracts.Profile;
using MediatR;

namespace Garage.Application.Profile.Commands.ChangePassword;

public sealed record ChangePasswordCommand(ChangePasswordRequest Request) : IRequest<Result<bool>>;
