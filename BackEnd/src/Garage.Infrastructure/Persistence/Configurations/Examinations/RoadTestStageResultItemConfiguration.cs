using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class RoadTestStageResultItemConfiguration : IEntityTypeConfiguration<RoadTestStageResultItem>
{
    public void Configure(EntityTypeBuilder<RoadTestStageResultItem> b)
    {
        b.ToTable("RoadTestStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.RoadTestIssueTypeId).IsRequired();
        b.Property(x => x.RoadTestIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
