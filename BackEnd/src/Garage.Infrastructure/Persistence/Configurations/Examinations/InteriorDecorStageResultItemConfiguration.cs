using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class InteriorDecorStageResultItemConfiguration : IEntityTypeConfiguration<InteriorDecorStageResultItem>
{
    public void Configure(EntityTypeBuilder<InteriorDecorStageResultItem> b)
    {
        b.ToTable("InteriorDecorStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.InsideAndDecorPartId).IsRequired();
        b.Property(x => x.InsideAndDecorPartIssueId).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
