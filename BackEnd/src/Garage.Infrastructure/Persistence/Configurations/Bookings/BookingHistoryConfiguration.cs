using Garage.Domain.Bookings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Bookings;

public sealed class BookingHistoryConfiguration : IEntityTypeConfiguration<BookingHistory>
{
    public void Configure(EntityTypeBuilder<BookingHistory> b)
    {
        b.ToTable("BookingHistory");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.BookingId).IsRequired();
        b.Property(x => x.Action).IsRequired();
        b.Property(x => x.Details).HasMaxLength(1000).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.HasIndex(x => x.BookingId);
    }
}
