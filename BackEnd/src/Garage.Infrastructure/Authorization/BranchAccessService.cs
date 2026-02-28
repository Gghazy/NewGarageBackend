using Garage.Application.Abstractions;
using Garage.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;

namespace Garage.Infrastructure.Authorization;

public sealed class BranchAccessService : IBranchAccessService
{
    private readonly ICurrentUserService _currentUser;
    private readonly IReadRepository<Employee> _employeeRepo;

    private bool _resolved;
    private IReadOnlyList<Guid>? _branchIds;

    public BranchAccessService(
        ICurrentUserService currentUser,
        IReadRepository<Employee> employeeRepo)
    {
        _currentUser = currentUser;
        _employeeRepo = employeeRepo;
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

        var result = await _employeeRepo.Query()
            .Where(e => e.UserId == userId)
            .Select(e => new
            {
                BranchIds = e.Branches.Select(b => b.BranchId).ToList()
            })
            .FirstOrDefaultAsync(ct);

        // null => no employee record => admin => unrestricted
        // empty list => employee with 0 branches => sees nothing
        _branchIds = result is null
            ? null
            : result.BranchIds.AsReadOnly();

        _resolved = true;
        return _branchIds;
    }
}
