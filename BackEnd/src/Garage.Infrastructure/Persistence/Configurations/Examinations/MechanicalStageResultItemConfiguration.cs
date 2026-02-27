using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class MechanicalStageResultItemConfiguration : IEntityTypeConfiguration<MechanicalStageResultItem>
{
    public void Configure(EntityTypeBuilder<MechanicalStageResultItem> b)
    {
        b.ToTable("MechanicalStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.MechIssueTypeId).IsRequired();
        b.Property(x => x.MechIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
