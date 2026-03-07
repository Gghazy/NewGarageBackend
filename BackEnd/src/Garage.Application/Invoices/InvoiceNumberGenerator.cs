using Garage.Application.Abstractions;
using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;

namespace Garage.Application.Invoices;

public sealed class InvoiceNumberGenerator(IReadRepository<Invoice> repo)
{
    public async Task<string> GenerateAsync(InvoiceType type, CancellationToken ct)
    {
        var year = DateTime.UtcNow.Year;
        var tag = type switch
        {
            InvoiceType.Refund     => "REF",
            InvoiceType.Adjustment => "ADJ",
            _                      => "INV"
        };
        var prefix = $"{tag}-{year}-";

        var lastNumber = await repo.QueryIncludingDeleted()
            .Where(i => i.InvoiceNumber != null && i.InvoiceNumber.StartsWith(prefix))
            .Select(i => i.InvoiceNumber!)
            .OrderByDescending(n => n)
            .FirstOrDefaultAsync(ct);

        var next = 1;
        if (lastNumber is not null)
        {
            var numPart = lastNumber.Substring(prefix.Length);
            if (int.TryParse(numPart, out var parsed))
                next = parsed + 1;
        }

        return $"{prefix}{next:D5}";
    }
}
