using Garage.Domain.Bookings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Bookings;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> b)
    {
        b.ToTable("Bookings");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ClientId);
        b.Property(x => x.ClientNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.ClientNameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.ClientPhone).IsRequired().HasMaxLength(50);

        b.Property(x => x.BranchId);
        b.Property(x => x.BranchNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.BranchNameEn).IsRequired().HasMaxLength(200);

        b.Property(x => x.ManufacturerId);
        b.Property(x => x.ManufacturerNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.ManufacturerNameEn).IsRequired().HasMaxLength(200);

        b.Property(x => x.CarMarkId);
        b.Property(x => x.CarMarkNameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.CarMarkNameEn).IsRequired().HasMaxLength(200);

        b.Property(x => x.Year);
        b.Property(x => x.Transmission).HasMaxLength(50);

        b.Property(x => x.ExaminationDate);
        b.Property(x => x.ExaminationTime);

        b.Property(x => x.Location).HasMaxLength(500);
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        b.Property(x => x.ConvertedExaminationId);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
