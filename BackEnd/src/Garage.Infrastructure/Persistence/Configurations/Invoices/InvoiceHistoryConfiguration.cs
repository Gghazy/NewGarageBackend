using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Invoices;

public sealed class InvoiceHistoryConfiguration : IEntityTypeConfiguration<InvoiceHistory>
{
    public void Configure(EntityTypeBuilder<InvoiceHistory> b)
    {
        b.ToTable("InvoiceHistory");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.InvoiceId).IsRequired();
        b.Property(x => x.Action).IsRequired();
        b.Property(x => x.Details).HasMaxLength(1000).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.HasIndex(x => x.InvoiceId);
    }
}
