using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Profile;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Profile.Queries.GetMyProfile;

public sealed class GetMyProfileQueryHandler(
    IApplicationDbContext db,
    ICurrentUserService currentUser)
    : BaseQueryHandler<GetMyProfileQuery, ProfileDto?>
{
    public override async Task<ProfileDto?> Handle(GetMyProfileQuery request, CancellationToken ct)
    {
        var userId = currentUser.UserId;

        var employee = await db.Employees.AsNoTracking()
            .Where(e => e.UserId == userId)
            .Select(e => new
            {
                e.NameAr,
                e.NameEn,
                BranchIds = e.Branches.Select(b => b.BranchId).ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (employee is null) return null;

        var user = await db.Users.AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new { u.Email, u.PhoneNumber })
            .FirstOrDefaultAsync(ct);

        var roleName = await (
            from ur in db.UserRoles.AsNoTracking()
            join r in db.Roles.AsNoTracking() on ur.RoleId equals r.Id
            where ur.UserId == userId
            select r.Name
        ).FirstOrDefaultAsync(ct) ?? "";

        var branches = await db.Branches.AsNoTracking()
            .Where(b => employee.BranchIds.Contains(b.Id))
            .Select(b => new ProfileBranchDto(b.Id, b.NameAr, b.NameEn))
            .ToListAsync(ct);

        return new ProfileDto(
            employee.NameAr,
            employee.NameEn,
            user?.Email ?? "",
            user?.PhoneNumber,
            roleName!,
            branches);
    }
}
