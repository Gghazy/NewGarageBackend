using Garage.Application.Abstractions;
using Garage.Application.Common;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Profile;
using Garage.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Profile.Commands.UpdateProfile;

public sealed class UpdateProfileHandler(
    ICurrentUserService currentUser,
    IRepository<Employee> employeeRepo,
    IUnitOfWork unitOfWork,
    IIdentityService identityService,
    IJwtTokenService jwtTokenService)
    : BaseCommandHandler<UpdateProfileCommand, UpdateProfileResponse>
{
    public override async Task<Result<UpdateProfileResponse>> Handle(
        UpdateProfileCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var employee = await employeeRepo.QueryTracking()
            .Include(e => e.Branches)
            .FirstOrDefaultAsync(e => e.UserId == userId, ct);

        if (employee is null)
            return Fail("Employee not found");

        employee.UpdateName(command.Request.NameAr, command.Request.NameEn);
        await unitOfWork.SaveChangesAsync(ct);

        var (succeeded, error) = await identityService.UpdateEmailAndPhoneAsync(
            userId, command.Request.Email, command.Request.PhoneNumber ?? "", ct);

        if (!succeeded)
            return Fail(error ?? "Failed to update user");

        var claims = await identityService.GetUserClaimsAsync(userId, ct);
        var branchIds = employee.Branches.Select(b => b.BranchId).ToList();

        var (token, expiresAt) = jwtTokenService.CreateToken(
            userId,
            command.Request.NameAr,
            command.Request.NameEn,
            command.Request.Email,
            claims,
            branchIds);

        return Ok(new UpdateProfileResponse(token, expiresAt));
    }
}
