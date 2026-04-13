using Garage.Application.Abstractions;
using Garage.Application.Common.Extensions;
using Garage.Domain.InvoiceManagement.Invoices;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices.Queries.GetRevenueComparison;

// ── DTOs ────────────────────────────────────────────────────────────────

public sealed record BranchRevenueDto(
    Guid BranchId,
    string BranchNameAr,
    string BranchNameEn,
    int InvoiceCount,
    decimal InvoiceTotal,
    int RefundCount,
    decimal RefundTotal,
    decimal NetRevenue,
    decimal DiscountTotal,
    string Currency
);

public sealed record PeriodSummaryDto(
    List<BranchRevenueDto> Branches,
    decimal GrandInvoiceTotal,
    decimal GrandRefundTotal,
    decimal GrandNetRevenue,
    decimal GrandDiscountTotal,
    int GrandInvoiceCount,
    int GrandRefundCount,
    string Currency
);

public sealed record RevenueComparisonResponse(
    PeriodSummaryDto Period1,
    PeriodSummaryDto Period2
);

// ── Query ───────────────────────────────────────────────────────────────

public sealed record GetRevenueComparisonQuery(
    DateTime From1,
    DateTime To1,
    DateTime From2,
    DateTime To2,
    Guid? BranchId
) : IRequest<RevenueComparisonResponse>;

// ── Handler ─────────────────────────────────────────────────────────────

public sealed class GetRevenueComparisonHandler(
    IReadRepository<Invoice> repo,
    IBranchAccessService branchAccess)
    : IRequestHandler<GetRevenueComparisonQuery, RevenueComparisonResponse>
{
    public async Task<RevenueComparisonResponse> Handle(
        GetRevenueComparisonQuery request, CancellationToken ct)
    {
        var accessibleBranches = await branchAccess.GetAccessibleBranchIdsAsync(ct);

        var period1 = await BuildPeriodSummary(
            accessibleBranches, request.From1, request.To1, request.BranchId, ct);
        var period2 = await BuildPeriodSummary(
            accessibleBranches, request.From2, request.To2, request.BranchId, ct);

        return new RevenueComparisonResponse(period1, period2);
    }

    private async Task<PeriodSummaryDto> BuildPeriodSummary(
        IReadOnlyList<Guid> accessibleBranches,
        DateTime from, DateTime to,
        Guid? branchId, CancellationToken ct)
    {
        var query = repo.Query()
            .Where(i => i.Status != InvoiceStatus.Cancelled && i.Status != InvoiceStatus.Draft)
            .Where(i => i.CreatedAtUtc >= from.Date)
            .Where(i => i.CreatedAtUtc < to.Date.AddDays(1))
            .WhereBranchAccessible(accessibleBranches);

        if (branchId.HasValue)
            query = query.Where(i => i.Branch.BranchId == branchId.Value);

        var grouped = await query
            .GroupBy(i => new
            {
                i.Branch.BranchId,
                i.Branch.NameAr,
                i.Branch.NameEn,
                i.Type
            })
            .Select(g => new
            {
                g.Key.BranchId,
                g.Key.NameAr,
                g.Key.NameEn,
                g.Key.Type,
                Count = g.Count(),
                Total = g.Sum(i => i.TotalWithTax.Amount),
                Discount = g.Sum(i => i.DiscountAmount.Amount)
            })
            .ToListAsync(ct);

        var branches = grouped
            .GroupBy(g => new { g.BranchId, g.NameAr, g.NameEn })
            .Select(bg =>
            {
                var invoices = bg.Where(x => x.Type == InvoiceType.Invoice);
                var refunds = bg.Where(x => x.Type == InvoiceType.Refund);

                var invoiceCount = invoices.Sum(x => x.Count);
                var invoiceTotal = invoices.Sum(x => x.Total);
                var discountTotal = invoices.Sum(x => x.Discount);
                var refundCount = refunds.Sum(x => x.Count);
                var refundTotal = refunds.Sum(x => x.Total);

                return new BranchRevenueDto(
                    bg.Key.BranchId, bg.Key.NameAr, bg.Key.NameEn,
                    invoiceCount, invoiceTotal,
                    refundCount, refundTotal,
                    invoiceTotal - refundTotal,
                    discountTotal, "SAR");
            })
            .OrderByDescending(b => b.NetRevenue)
            .ToList();

        return new PeriodSummaryDto(
            branches,
            branches.Sum(b => b.InvoiceTotal),
            branches.Sum(b => b.RefundTotal),
            branches.Sum(b => b.NetRevenue),
            branches.Sum(b => b.DiscountTotal),
            branches.Sum(b => b.InvoiceCount),
            branches.Sum(b => b.RefundCount),
            "SAR");
    }
}
