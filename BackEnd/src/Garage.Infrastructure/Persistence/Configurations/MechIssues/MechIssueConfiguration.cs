using Garage.Domain.MechIssues.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.MechIssues;

public class MechIssueConfiguration : IEntityTypeConfiguration<MechIssue>
{
    public void Configure(EntityTypeBuilder<MechIssue> b)
    {
        b.ToTable("MechIssues");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");


    }
}

