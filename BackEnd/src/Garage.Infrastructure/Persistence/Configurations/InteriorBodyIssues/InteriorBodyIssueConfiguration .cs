using Garage.Domain.InteriorBodyIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.InteriorBodyIssues;

public class InteriorBodyIssueConfiguration : IEntityTypeConfiguration<InteriorBodyIssue>
{
    public void Configure(EntityTypeBuilder<InteriorBodyIssue> b)
    {
        b.ToTable("InteriorBodyIssues");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}

