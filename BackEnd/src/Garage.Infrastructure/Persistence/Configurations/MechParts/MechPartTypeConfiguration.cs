using Garage.Domain.MechPartTypes.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.MechParts;

public class MechPartTypeConfiguration : IEntityTypeConfiguration<MechPartType>
{
    public void Configure(EntityTypeBuilder<MechPartType> b)
    {
        b.ToTable("MechPartTypes");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");


        b.HasMany(x => x.MechParts)
             .WithOne(x => x.MechPartType)
             .HasForeignKey(x => x.MechPartTypeId)
             .OnDelete(DeleteBehavior.Restrict);
    }
}
