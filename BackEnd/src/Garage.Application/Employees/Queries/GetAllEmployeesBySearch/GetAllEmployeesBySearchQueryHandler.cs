using Garage.Application.Abstractions;
using Garage.Contracts.Common;
using Garage.Contracts.Employees;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Employees.Queries.GetAllEmployeesBySearch;

public class GetAllEmployeesBySearchQueryHandler(IApplicationDbContext _context)
    : IRequestHandler<GetAllEmployeesBySearchQuery, QueryResult<EmployeeDto>>
{
    public async Task<QueryResult<EmployeeDto>> Handle(GetAllEmployeesBySearchQuery request, CancellationToken ct)
    {
        var protectedRoles = new[] { "Admin", "Manager" };

        var baseQuery =
            from e in _context.Employees.AsNoTracking()
            join u in _context.Users.AsNoTracking() on e.UserId equals u.Id
            join ur in _context.UserRoles.AsNoTracking() on u.Id equals ur.UserId
            join r in _context.Roles.AsNoTracking() on ur.RoleId equals r.Id
            where !protectedRoles.Contains(r.Name)
            select new
            {
                e.Id,
                e.UserId,
                e.NameAr,
                e.NameEn,
                RoleId = r.Id,
                RoleName = r.Name,
                u.PhoneNumber,
                IsActive = !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTime.UtcNow,
                u.Email
            };

        var count = await baseQuery.CountAsync(ct);

        var page = request.Search.CurrentPage < 1 ? 1 : request.Search.CurrentPage;
        var size = request.Search.ItemsPerPage < 1 ? 10 : request.Search.ItemsPerPage;

        var employees = await baseQuery
            .OrderByDescending(e => e.NameAr)
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(ct);

        var employeeIds = employees.Select(e => e.Id).ToList();

        var branchesLookup = await _context.Employees.AsNoTracking()
            .Where(e => employeeIds.Contains(e.Id))
            .SelectMany(e => e.Branches, (e, eb) => new { e.Id, eb.BranchId })
            .Join(_context.Branches.AsNoTracking(),
                eb => eb.BranchId,
                b => b.Id,
                (eb, b) => new { EmployeeId = eb.Id, BranchId = b.Id, BranchNameAr = b.NameAr, BranchNameEn = b.NameEn })
            .ToListAsync(ct);

        var grouped = branchesLookup
            .GroupBy(x => x.EmployeeId)
            .ToDictionary(
                g => g.Key,
                g => g.Select(x => new EmployeeBranchDto(x.BranchId, x.BranchNameAr, x.BranchNameEn)).ToList());

        var items = employees.Select(e => new EmployeeDto(
            e.Id,
            e.UserId,
            e.NameAr,
            e.NameEn,
            e.RoleId,
            e.RoleName,
            e.PhoneNumber,
            grouped.GetValueOrDefault(e.Id, new List<EmployeeBranchDto>()),
            e.IsActive,
            e.Email
        )).ToList();

        return new QueryResult<EmployeeDto>(items, count, page, size);
    }
}
