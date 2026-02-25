using Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("ExaminationPayments");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // shadow FK set by ExaminationConfiguration
        b.Property<Guid>("ExaminationId").IsRequired();

        b.Property(x => x.Method).IsRequired();
        b.Property(x => x.Type).IsRequired().HasDefaultValue(PaymentType.Payment);
        b.Property(x => x.Notes).HasMaxLength(500).IsRequired(false);

        // ── Amount (owned Money) ──────────────────────────────────────────────
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

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
