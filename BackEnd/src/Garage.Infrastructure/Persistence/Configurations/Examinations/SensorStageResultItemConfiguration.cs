using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class SensorStageResultItemConfiguration : IEntityTypeConfiguration<SensorStageResultItem>
{
    public void Configure(EntityTypeBuilder<SensorStageResultItem> b)
    {
        b.ToTable("SensorStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.SensorIssueId).IsRequired();
        b.Property(x => x.Evaluation).HasMaxLength(20).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
