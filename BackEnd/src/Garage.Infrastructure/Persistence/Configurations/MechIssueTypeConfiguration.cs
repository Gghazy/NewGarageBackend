using Garage.Domain.MechIssueTypes.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations;

public class MechIssueTypeConfiguration : IEntityTypeConfiguration<MechIssueType>
{
    public void Configure(EntityTypeBuilder<MechIssueType> b)
    {
        b.ToTable("MechIssueTypes");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");


        b.HasMany(x => x.MechIssues)
             .WithOne(x => x.MechIssueType)
             .HasForeignKey(x => x.MechIssueTypeId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}

