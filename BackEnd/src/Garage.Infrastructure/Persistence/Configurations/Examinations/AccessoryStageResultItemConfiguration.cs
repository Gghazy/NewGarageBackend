using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class AccessoryStageResultItemConfiguration : IEntityTypeConfiguration<AccessoryStageResultItem>
{
    public void Configure(EntityTypeBuilder<AccessoryStageResultItem> b)
    {
        b.ToTable("AccessoryStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.AccessoryPartId).IsRequired();
        b.Property(x => x.AccessoryIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
