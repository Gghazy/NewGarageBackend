using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;

namespace Garage.Application.Profile.Commands.ChangePassword;

public sealed class ChangePasswordHandler(
    ICurrentUserService currentUser,
    IIdentityService identityService)
    : BaseCommandHandler<ChangePasswordCommand, bool>
{
    public override async Task<Result<bool>> Handle(
        ChangePasswordCommand command, CancellationToken ct)
    {
        var (succeeded, error) = await identityService.ChangePasswordAsync(
            currentUser.UserId,
            command.Request.CurrentPassword,
            command.Request.NewPassword,
            ct);

        return succeeded ? Ok(true) : Fail(error ?? "Failed to change password");
    }
}
