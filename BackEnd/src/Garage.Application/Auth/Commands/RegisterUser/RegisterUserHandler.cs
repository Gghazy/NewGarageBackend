using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;

namespace Garage.Application.Auth.Commands.RegisterUser;

public class RegisterUserHandler : BaseCommandHandler<RegisterUserCommand, Guid?>
{
    private readonly IIdentityService _identityService;

    public RegisterUserHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public override async Task<Result<Guid?>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        var (succeeded, error, userId) = await _identityService.CreateUserAsync(request.Request, ct);
        return succeeded
            ? Ok(userId)
            : Fail(error ?? "Failed to create user");
    }
}

