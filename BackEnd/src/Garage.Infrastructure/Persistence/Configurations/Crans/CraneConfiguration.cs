using Garage.Domain.Cranes.Entity;
using Garage.Domain.SensorIssues.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Crans;

public class CraneConfiguration : IEntityTypeConfiguration<Crane>
{
    public void Configure(EntityTypeBuilder<Crane> b)
    {
        b.ToTable("Cranes");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}

