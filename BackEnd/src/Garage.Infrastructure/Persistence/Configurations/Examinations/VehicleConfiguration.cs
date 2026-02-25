using Garage.Domain.ExaminationManagement.Vehicles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> b)
    {
        b.ToTable("Vehicles");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // ── Manufacturer ──────────────────────────────────────────────────────
        b.Property(x => x.ManufacturerId).IsRequired();
        b.Property(x => x.ManufacturerNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.ManufacturerNameEn).IsRequired().HasMaxLength(200);

        // ── Car mark ──────────────────────────────────────────────────────────
        b.Property(x => x.CarMarkId).IsRequired();
        b.Property(x => x.CarMarkNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.CarMarkNameEn).IsRequired().HasMaxLength(200);

        // ── Basic info ────────────────────────────────────────────────────────
        b.Property(x => x.Year).IsRequired(false);
        b.Property(x => x.Color).HasMaxLength(50).IsRequired(false);
        b.Property(x => x.Vin).HasMaxLength(50).IsRequired(false);

        // ── Plate ─────────────────────────────────────────────────────────────
        b.Property(x => x.HasPlate).IsRequired();

        b.OwnsOne(x => x.Plate, plate =>
        {
            plate.Property(p => p.Letters)
                .HasColumnName("PlateLetters")
                .HasMaxLength(10)
                .IsRequired(false);

            plate.Property(p => p.Numbers)
                .HasColumnName("PlateNumbers")
                .HasMaxLength(10)
                .IsRequired(false);
        });

        // ── Odometer ──────────────────────────────────────────────────────────
        b.Property(x => x.Mileage).HasColumnType("decimal(10,2)").IsRequired(false);
        b.Property(x => x.MileageUnit).IsRequired();

        // ── Transmission ──────────────────────────────────────────────────────
        b.Property(x => x.Transmission).IsRequired(false);

        // ── Audit ─────────────────────────────────────────────────────────────
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
