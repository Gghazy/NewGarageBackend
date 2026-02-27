using Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class ExaminationConfiguration : IEntityTypeConfiguration<Examination>
{
    public void Configure(EntityTypeBuilder<Examination> b)
    {
        b.ToTable("Examinations");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // ── Status & type ─────────────────────────────────────────────────────
        b.Property(x => x.Status).IsRequired();
        b.Property(x => x.Type).IsRequired();

        // ── Flags ─────────────────────────────────────────────────────────────
        b.Property(x => x.HasWarranty).IsRequired();
        b.Property(x => x.HasPhotos).IsRequired();
        b.Property(x => x.MarketerCode).HasMaxLength(50).IsRequired(false);
        b.Property(x => x.Notes).HasMaxLength(1000).IsRequired(false);

        // ── Client reference (owned) ──────────────────────────────────────────
        b.OwnsOne(x => x.Client, c =>
        {
            c.Property(p => p.ClientId)
                .HasColumnName("ClientId")
                .IsRequired();

            c.Property(p => p.NameAr)
                .HasColumnName("ClientNameAr")
                .HasMaxLength(200)
                .IsRequired();

            c.Property(p => p.NameEn)
                .HasColumnName("ClientNameEn")
                .HasMaxLength(200)
                .IsRequired();

            c.Property(p => p.PhoneNumber)
                .HasColumnName("ClientPhone")
                .HasMaxLength(20)
                .IsRequired();

            c.Property(p => p.Email)
                .HasColumnName("ClientEmail")
                .HasMaxLength(200)
                .IsRequired(false);
        });

        // ── Branch reference (owned) ──────────────────────────────────────────
        b.OwnsOne(x => x.Branch, br =>
        {
            br.Property(p => p.BranchId)
                .HasColumnName("BranchId")
                .IsRequired();

            br.Property(p => p.NameAr)
                .HasColumnName("BranchNameAr")
                .HasMaxLength(200)
                .IsRequired();

            br.Property(p => p.NameEn)
                .HasColumnName("BranchNameEn")
                .HasMaxLength(200)
                .IsRequired();
        });

        // ── Vehicle snapshot (owned) ──────────────────────────────────────────
        b.OwnsOne(x => x.Vehicle, v =>
        {
            v.Property(p => p.VehicleId)
                .HasColumnName("VehicleId")
                .IsRequired();

            v.Property(p => p.ManufacturerId)
                .HasColumnName("VehicleManufacturerId")
                .IsRequired();

            v.Property(p => p.ManufacturerNameAr)
                .HasColumnName("VehicleManufacturerNameAr")
                .HasMaxLength(200)
                .IsRequired();

            v.Property(p => p.ManufacturerNameEn)
                .HasColumnName("VehicleManufacturerNameEn")
                .HasMaxLength(200)
                .IsRequired();

            v.Property(p => p.CarMarkId)
                .HasColumnName("VehicleCarMarkId")
                .IsRequired();

            v.Property(p => p.CarMarkNameAr)
                .HasColumnName("VehicleCarMarkNameAr")
                .HasMaxLength(200)
                .IsRequired();

            v.Property(p => p.CarMarkNameEn)
                .HasColumnName("VehicleCarMarkNameEn")
                .HasMaxLength(200)
                .IsRequired();

            v.Property(p => p.Year)
                .HasColumnName("VehicleYear")
                .IsRequired(false);

            v.Property(p => p.Color)
                .HasColumnName("VehicleColor")
                .HasMaxLength(50)
                .IsRequired(false);

            v.Property(p => p.Vin)
                .HasColumnName("VehicleVin")
                .HasMaxLength(50)
                .IsRequired(false);

            v.Property(p => p.HasPlate)
                .HasColumnName("VehicleHasPlate")
                .IsRequired();

            v.OwnsOne(p => p.Plate, plate =>
            {
                plate.Property(pp => pp.Letters)
                    .HasColumnName("VehiclePlateLetters")
                    .HasMaxLength(10)
                    .IsRequired(false);

                plate.Property(pp => pp.Numbers)
                    .HasColumnName("VehiclePlateNumbers")
                    .HasMaxLength(10)
                    .IsRequired(false);
            });

            v.Property(p => p.Mileage)
                .HasColumnName("VehicleMileage")
                .HasColumnType("decimal(10,2)")
                .IsRequired(false);

            v.Property(p => p.MileageUnit)
                .HasColumnName("VehicleMileageUnit")
                .IsRequired();

            v.Property(p => p.Transmission)
                .HasColumnName("VehicleTransmission")
                .IsRequired(false);
        });

        // ── Items ─────────────────────────────────────────────────────────────
        b.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("ExaminationId")
            .OnDelete(DeleteBehavior.Cascade);

        // ── Sensor Stage Result ──────────────────────────────────────────────
        b.HasOne(x => x.SensorStageResult)
            .WithOne()
            .HasForeignKey<SensorStageResult>(s => s.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Dashboard Indicators Stage Result ───────────────────────────────
        b.HasOne(x => x.DashboardIndicatorsStageResult)
            .WithOne()
            .HasForeignKey<DashboardIndicatorsStageResult>(s => s.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Interior Decor Stage Result ─────────────────────────────────────
        b.HasOne(x => x.InteriorDecorStageResult)
            .WithOne()
            .HasForeignKey<InteriorDecorStageResult>(s => s.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Interior Body Stage Result ──────────────────────────────────────
        b.HasOne(x => x.InteriorBodyStageResult)
            .WithOne()
            .HasForeignKey<InteriorBodyStageResult>(s => s.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Exterior Body Stage Result ──────────────────────────────────────
        b.HasOne(x => x.ExteriorBodyStageResult)
            .WithOne()
            .HasForeignKey<ExteriorBodyStageResult>(s => s.ExaminationId)
            .OnDelete(DeleteBehavior.Cascade);

        // ── Audit ─────────────────────────────────────────────────────────────
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
