using Garage.Domain.RoadTestIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.RoadTestIssues;

public sealed class RoadTestIssueTypeConfiguration
    : IEntityTypeConfiguration<RoadTestIssueType>
{
    public void Configure(EntityTypeBuilder<RoadTestIssueType> builder)
    {
        builder.ToTable("RoadTestIssueTypes");

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

        builder.HasMany(x => x.RoadTestIssues)
               .WithOne(x => x.RoadTestIssueType)
               .HasForeignKey(x => x.RoadTestIssueTypeId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
