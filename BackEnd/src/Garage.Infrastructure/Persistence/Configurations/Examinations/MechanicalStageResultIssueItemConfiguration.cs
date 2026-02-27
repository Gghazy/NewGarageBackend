using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class MechanicalStageResultIssueItemConfiguration : IEntityTypeConfiguration<MechanicalStageResultIssueItem>
{
    public void Configure(EntityTypeBuilder<MechanicalStageResultIssueItem> b)
    {
        b.ToTable("MechanicalStageResultIssueItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.MechPartId).IsRequired();
        b.Property(x => x.MechIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
