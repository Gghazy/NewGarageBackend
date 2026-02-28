using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Domain.InvoiceManagement.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetRevenue;

public sealed record RevenueDto(
    decimal NetRevenue,
    decimal TotalDiscounts,
    int RefundCount,
    decimal RefundAmount,
    string Currency
);

public sealed record GetRevenueQuery(DateTime? From, DateTime? To, Guid? BranchId) : IRequest<RevenueDto>;

public sealed class GetRevenueHandler(
    IReadRepository<Invoice> invoiceRepo,
    IBranchAccessService branchAccess)
    : IRequestHandler<GetRevenueQuery, RevenueDto>
{
    public async Task<RevenueDto> Handle(GetRevenueQuery request, CancellationToken ct)
    {
        var accessibleBranches = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var query = invoiceRepo.Query()
            .Where(i => i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Draft);

        if (request.From.HasValue)
            query = query.Where(i => i.CreatedAtUtc >= request.From.Value);
        if (request.To.HasValue)
            query = query.Where(i => i.CreatedAtUtc < request.To.Value.AddDays(1));

        query = query.WhereBranchAccessible(accessibleBranches);
        if (request.BranchId.HasValue)
            query = query.Where(i => i.Branch.BranchId == request.BranchId.Value);

        // Regular invoices total
        var invoiceTotal = await query
            .Where(i => i.Type == InvoiceType.Invoice)
            .SumAsync(i => i.TotalWithTax.Amount, ct);

        // Total discounts on regular invoices
        var totalDiscounts = await query
            .Where(i => i.Type == InvoiceType.Invoice)
            .SumAsync(i => i.DiscountAmount.Amount, ct);

        // Refund invoices count & total
        var refundData = await query
            .Where(i => i.Type == InvoiceType.Refund)
            .GroupBy(i => 1)
            .Select(g => new { Count = g.Count(), Amount = g.Sum(i => i.TotalWithTax.Amount) })
            .FirstOrDefaultAsync(ct);

        var refundCount = refundData?.Count ?? 0;
        var refundAmount = refundData?.Amount ?? 0;

        var netRevenue = invoiceTotal - refundAmount;

        return new RevenueDto(netRevenue, totalDiscounts, refundCount, refundAmount, "SAR");
    }
}
