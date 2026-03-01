using Garage.Application.Abstractions;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Garage.Infrastructure.Authorization;

public sealed class BranchAccessService : IBranchAccessService
{
    private static readonly string[] UnrestrictedRoles = ["Admin", "Manager"];

    private readonly ICurrentUserService _currentUser;
    private readonly IReadRepository<Employee> _employeeRepo;
    private readonly UserManager<AppUser> _userManager;

    private bool _resolved;
    private IReadOnlyList<Guid>? _branchIds;

    public BranchAccessService(
        ICurrentUserService currentUser,
        IReadRepository<Employee> employeeRepo,
        UserManager<AppUser> userManager)
    {
        _currentUser = currentUser;
        _employeeRepo = employeeRepo;
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<Guid>?> GetAccessibleBranchIdsAsync(CancellationToken ct = default)
    {
        if (_resolved) return _branchIds;

        var userId = _currentUser.UserId;
        if (userId == Guid.Empty)
        {
            _resolved = true;
            _branchIds = null;
            return _branchIds;
        }

        // Users with Admin or Manager role see all branches
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user is not null)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any(r => UnrestrictedRoles.Contains(r, StringComparer.OrdinalIgnoreCase)))
            {
                _resolved = true;
                _branchIds = null;
                return _branchIds;
            }
        }

        var result = await _employeeRepo.Query()
            .Where(e => e.UserId == userId)
            .Select(e => new
            {
                BranchIds = e.Branches.Select(b => b.BranchId).ToList()
            })
            .FirstOrDefaultAsync(ct);

        // null => no employee record => unrestricted
        // empty list => employee with 0 branches => sees nothing
        _branchIds = result is null
            ? null
            : result.BranchIds.AsReadOnly();

        _resolved = true;
        return _branchIds;
    }
}
