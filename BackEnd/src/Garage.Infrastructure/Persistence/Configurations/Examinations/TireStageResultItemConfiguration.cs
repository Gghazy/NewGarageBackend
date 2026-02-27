using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class TireStageResultItemConfiguration : IEntityTypeConfiguration<TireStageResultItem>
{
    public void Configure(EntityTypeBuilder<TireStageResultItem> b)
    {
        b.ToTable("TireStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Position).HasMaxLength(20).IsRequired();
        b.Property(x => x.Year).IsRequired(false);
        b.Property(x => x.Week).IsRequired(false);
        b.Property(x => x.Condition).HasMaxLength(50).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
