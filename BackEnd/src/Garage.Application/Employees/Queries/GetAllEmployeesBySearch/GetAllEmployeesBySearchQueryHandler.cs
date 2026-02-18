

using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Contracts.Common;
using Garage.Contracts.Employees;
using Garage.Contracts.MechIssues;
using Garage.Domain.Employees.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Employees.Queries.GetAllEmployeesBySearch;

public class GetAllEmployeesBySearchQueryHandler(IApplicationDbContext _context) : IRequestHandler<GetAllEmployeesBySearchQuery, QueryResult<EmployeeDto>>
{
    public async Task<QueryResult<EmployeeDto>> Handle(GetAllEmployeesBySearchQuery request, CancellationToken ct)
    {
        var q =
       from e in _context.Employees.AsNoTracking()

       join u in _context.Users.AsNoTracking()
           on e.UserId equals u.Id

       join b in _context.Branches.AsNoTracking()
           on e.BranchId equals b.Id

       join ur in _context.UserRoles.AsNoTracking()
           on u.Id equals ur.UserId

       join r in _context.Roles.AsNoTracking()
           on ur.RoleId equals r.Id

       select new EmployeeDto(
           e.Id,
           e.UserId,
           e.NameAr,
           e.NameEn,
           r.Id,
           r.Name,
           u.PhoneNumber,
           b.Id,
           b.NameEn,
           b.NameAr,
           !u.LockoutEnabled || u.LockoutEnd == null || u.LockoutEnd < DateTime.UtcNow,
           u.Email
       );

        return await q.ToQueryResult(request.Search.CurrentPage,request.Search.ItemsPerPage);
    }
}

