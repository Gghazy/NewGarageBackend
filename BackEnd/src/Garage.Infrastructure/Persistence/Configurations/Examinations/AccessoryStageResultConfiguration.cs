using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class AccessoryStageResultConfiguration : IEntityTypeConfiguration<AccessoryStageResult>
{
    public void Configure(EntityTypeBuilder<AccessoryStageResult> b)
    {
        b.ToTable("AccessoryStageResults");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ExaminationId).IsRequired();

        b.Property(x => x.NoIssuesFound).IsRequired();
        b.Property(x => x.Comments).HasMaxLength(2000).IsRequired(false);

        b.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("AccessoryStageResultId")
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
