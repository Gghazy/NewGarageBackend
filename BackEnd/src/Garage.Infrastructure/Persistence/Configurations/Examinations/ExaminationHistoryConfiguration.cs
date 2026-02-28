using Garage.Domain.ExaminationManagement.Examinations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Examinations;

public sealed class ExaminationHistoryConfiguration : IEntityTypeConfiguration<ExaminationHistory>
{
    public void Configure(EntityTypeBuilder<ExaminationHistory> b)
    {
        b.ToTable("ExaminationHistory");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.ExaminationId).IsRequired();
        b.Property(x => x.Action).IsRequired();
        b.Property(x => x.Details).HasMaxLength(1000).IsRequired(false);

        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.HasIndex(x => x.ExaminationId);
    }
}
