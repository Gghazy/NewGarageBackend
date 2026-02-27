using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class TireStageResultConfiguration : IEntityTypeConfiguration<TireStageResult>
{
    public void Configure(EntityTypeBuilder<TireStageResult> b)
    {
        b.ToTable("TireStageResults");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ExaminationId).IsRequired();

        b.Property(x => x.NoIssuesFound).IsRequired();
        b.Property(x => x.Comments).HasMaxLength(2000).IsRequired(false);

        b.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("TireStageResultId")
            .OnDelete(DeleteBehavior.Cascade);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
