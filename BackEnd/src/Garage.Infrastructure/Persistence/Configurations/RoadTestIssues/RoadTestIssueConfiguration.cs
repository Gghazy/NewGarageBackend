using Garage.Domain.RoadTestIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.RoadTestIssues;

public sealed class RoadTestIssueConfiguration
    : IEntityTypeConfiguration<RoadTestIssue>
{
    public void Configure(EntityTypeBuilder<RoadTestIssue> builder)
    {
        builder.ToTable("RoadTestIssues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedNever();

        builder.Property(x => x.NameAr)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.NameEn)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
               .HasDefaultValueSql("GETUTCDATE()");
    }
}
