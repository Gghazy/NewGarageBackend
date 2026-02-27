using Garage.Domain.MechParts.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.MechParts;

public class MechPartConfiguration : IEntityTypeConfiguration<MechPart>
{
    public void Configure(EntityTypeBuilder<MechPart> b)
    {
        b.ToTable("MechParts");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");


    }
}
