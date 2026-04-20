using Garage.Domain.ServiceDiscounts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.ServiceDiscounts;

public class ServiceDiscountConfiguration : IEntityTypeConfiguration<ServiceDiscount>
{
    public void Configure(EntityTypeBuilder<ServiceDiscount> b)
    {
        b.ToTable("ServiceDiscounts");

        b.HasKey(x => x.Id);

        b.Property(x => x.ServiceId)
            .IsRequired();

        b.Property(x => x.DiscountPercent)
            .HasPrecision(5, 2)
            .IsRequired();

        b.Property(x => x.StartDate)
            .IsRequired();

        b.Property(x => x.EndDate)
            .IsRequired();

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        b.HasIndex(x => x.ServiceId);

        b.HasIndex(x => new { x.ServiceId, x.StartDate, x.EndDate });

        b.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_ServiceDiscount_DateRange",
                "[StartDate] <= [EndDate]");

            t.HasCheckConstraint(
                "CK_ServiceDiscount_Percent",
                "[DiscountPercent] > 0 AND [DiscountPercent] <= 100");
        });
    }
}
