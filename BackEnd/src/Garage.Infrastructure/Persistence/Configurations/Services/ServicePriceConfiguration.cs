using Garage.Domain.ServicePrices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Services;
public class ServicePriceConfiguration : IEntityTypeConfiguration<ServicePrice>
{
    public void Configure(EntityTypeBuilder<ServicePrice> b)
    {
        b.ToTable("ServicePrices");

        b.HasKey(x => x.Id);

        b.Property(x => x.ServiceId)
            .IsRequired();

        b.Property(x => x.MarkId)
            .IsRequired();

        b.Property(x => x.FromYear)
            .IsRequired();

        b.Property(x => x.ToYear)
            .IsRequired();

        b.Property(x => x.Price)
            .HasPrecision(18, 2)
            .IsRequired();






        b.HasIndex(x => new { x.ServiceId, x.MarkId });

        b.HasIndex(x => new { x.ServiceId, x.MarkId, x.FromYear, x.ToYear });

        b.HasIndex(x => new { x.ServiceId, x.MarkId, x.FromYear, x.ToYear })
            .IsUnique();


        b.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_ServicePrice_YearRange",
                "[FromYear] <= [ToYear]");

            t.HasCheckConstraint(
                "CK_ServicePrice_Price",
                "[Price] > 0");
        });
    }


}


