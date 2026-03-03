using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Invoices;

public sealed class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> b)
    {
        b.ToTable("InvoiceItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Description).HasMaxLength(500).IsRequired();

        b.OwnsOne(x => x.UnitPrice, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.OwnsOne(x => x.TotalPrice, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("TotalPriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("TotalPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.Property(x => x.ServiceId).IsRequired(false);
        b.Property(x => x.ServiceNameAr).HasMaxLength(200).IsRequired(false);
        b.Property(x => x.ServiceNameEn).HasMaxLength(200).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
