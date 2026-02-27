using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class DashboardIndicatorsStageResultItemConfiguration : IEntityTypeConfiguration<DashboardIndicatorsStageResultItem>
{
    public void Configure(EntityTypeBuilder<DashboardIndicatorsStageResultItem> b)
    {
        b.ToTable("DashboardIndicatorsStageResultItems");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.Key).HasMaxLength(50).IsRequired();
        b.Property(x => x.Value).HasColumnType("decimal(18,2)").IsRequired(false);
        b.Property(x => x.NotApplicable).IsRequired();

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
