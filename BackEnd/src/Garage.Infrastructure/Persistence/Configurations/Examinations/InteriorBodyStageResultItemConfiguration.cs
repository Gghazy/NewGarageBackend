using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class InteriorBodyStageResultItemConfiguration : IEntityTypeConfiguration<InteriorBodyStageResultItem>
{
    public void Configure(EntityTypeBuilder<InteriorBodyStageResultItem> b)
    {
        b.ToTable("InteriorBodyStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.InteriorBodyPartId).IsRequired();
        b.Property(x => x.InteriorBodyIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
