using Garage.Application.Abstractions;
using Garage.Application.Common.Handlers;
using Garage.Contracts.Invoices;
using Garage.Domain.Employees.Entities;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetHistory;

public sealed class GetInvoiceHistoryQueryHandler(
    IReadRepository<InvoiceHistory> historyRepo,
    IReadRepository<Employee> employeeRepo,
    IApplicationDbContext dbContext)
    : BaseQueryHandler<GetInvoiceHistoryQuery, List<InvoiceHistoryDto>>
{
    public override async Task<List<InvoiceHistoryDto>> Handle(
        GetInvoiceHistoryQuery request, CancellationToken ct)
    {
        var history = await historyRepo.Query()
            .Where(h => h.InvoiceId == request.InvoiceId)
            .OrderByDescending(h => h.CreatedAtUtc)
            .ToListAsync(ct);

        var userIds = history
            .Where(h => h.CreatedBy.HasValue)
            .Select(h => h.CreatedBy!.Value)
            .Distinct()
            .ToList();

        var employees = await employeeRepo.Query()
            .Where(e => userIds.Contains(e.UserId))
            .Select(e => new { e.UserId, e.NameAr, e.NameEn })
            .ToListAsync(ct);

        var employeeLookup = employees.ToDictionary(e => e.UserId);

        var missingIds = userIds.Except(employeeLookup.Keys).ToList();
        var usernameLookup = new Dictionary<Guid, string>();
        if (missingIds.Count > 0)
        {
            usernameLookup = await dbContext.Users
                .Where(u => missingIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.UserName ?? "", ct);
        }

        return history.Select(h =>
        {
            string? nameAr = null, nameEn = null;
            if (h.CreatedBy.HasValue)
            {
                if (employeeLookup.TryGetValue(h.CreatedBy.Value, out var emp))
                {
                    nameAr = emp.NameAr;
                    nameEn = emp.NameEn;
                }
                else if (usernameLookup.TryGetValue(h.CreatedBy.Value, out var username))
                {
                    nameAr = username;
                    nameEn = username;
                }
            }

            return new InvoiceHistoryDto(
                h.Id,
                h.Action.ToString(),
                h.Details,
                h.CreatedBy,
                nameAr,
                nameEn,
                h.CreatedAtUtc
            );
        }).ToList();
    }
}
