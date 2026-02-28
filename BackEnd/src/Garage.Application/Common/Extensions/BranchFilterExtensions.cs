using Domain.ExaminationManagement.Examinations;
using Garage.Domain.InvoiceManagement.Invoices;

namespace Garage.Application.Common.Extensions;

public static class BranchFilterExtensions
{
    public static IQueryable<Examination> WhereBranchAccessible(
        this IQueryable<Examination> query,
        IReadOnlyList<Guid>? branchIds)
    {
        if (branchIds is null) return query;
        return query.Where(e => branchIds.Contains(e.Branch.BranchId));
    }

    public static IQueryable<Invoice> WhereBranchAccessible(
        this IQueryable<Invoice> query,
        IReadOnlyList<Guid>? branchIds)
    {
        if (branchIds is null) return query;
        return query.Where(i => branchIds.Contains(i.Branch.BranchId));
    }
}
