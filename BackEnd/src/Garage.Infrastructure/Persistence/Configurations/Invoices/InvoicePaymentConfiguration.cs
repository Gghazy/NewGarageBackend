using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Invoices;

public sealed class InvoicePaymentConfiguration : IEntityTypeConfiguration<InvoicePayment>
{
    public void Configure(EntityTypeBuilder<InvoicePayment> b)
    {
        b.ToTable("InvoicePayments");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.OwnsOne(x => x.Amount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.Property(x => x.MethodId).IsRequired();
        b.Property(x => x.MethodNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.MethodNameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.Notes).HasMaxLength(500).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
