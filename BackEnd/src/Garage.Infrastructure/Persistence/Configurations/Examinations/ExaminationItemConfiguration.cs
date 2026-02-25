using Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class ExaminationItemConfiguration : IEntityTypeConfiguration<ExaminationItem>
{
    public void Configure(EntityTypeBuilder<ExaminationItem> b)
    {
        b.ToTable("ExaminationItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // shadow FK set by ExaminationConfiguration
        b.Property<Guid>("ExaminationId").IsRequired();

        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(1000).IsRequired(false);

        // ── Service snapshot (owned) ──────────────────────────────────────────
        b.OwnsOne(x => x.Service, s =>
        {
            s.Property(p => p.ServiceId)
                .HasColumnName("ServiceId")
                .IsRequired();

            s.Property(p => p.NameAr)
                .HasColumnName("ServiceNameAr")
                .HasMaxLength(200)
                .IsRequired();

            s.Property(p => p.NameEn)
                .HasColumnName("ServiceNameEn")
                .HasMaxLength(200)
                .IsRequired();

            s.OwnsOne(p => p.DefaultPrice, m =>
            {
                m.Property(pp => pp.Amount)
                    .HasColumnName("DefaultPriceAmount")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                m.Property(pp => pp.Currency)
                    .HasColumnName("DefaultPriceCurrency")
                    .HasMaxLength(3)
                    .IsRequired();
            });
        });

        // ── Actual price (owned Money) ────────────────────────────────────────
        b.OwnsOne(x => x.Price, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("PriceAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("PriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
