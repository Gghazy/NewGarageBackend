using Garage.Domain.ServicePointRules.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.ServicePointRules;

public class ServicePointRuleConfiguration : IEntityTypeConfiguration<ServicePointRule>
{
    public void Configure(EntityTypeBuilder<ServicePointRule> b)
    {
        b.ToTable("ServicePointRules");

        b.HasKey(x => x.Id);

        b.Property(x => x.FromAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.ToAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        b.Property(x => x.Points).IsRequired();

        b.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        b.HasIndex(x => new { x.FromAmount, x.ToAmount });

        b.ToTable(t =>
        {
            t.HasCheckConstraint("CK_ServicePointRule_AmountRange", "[FromAmount] < [ToAmount]");
            t.HasCheckConstraint("CK_ServicePointRule_Points", "[Points] > 0");
        });
    }
}
