using Garage.Domain.SensorIssues.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations;

public class SensorIssueConfiguration : IEntityTypeConfiguration<SensorIssue>
{
    public void Configure(EntityTypeBuilder<SensorIssue> b)
    {
        b.ToTable("SensorIssues");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.Code).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}

