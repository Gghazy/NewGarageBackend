using Garage.Domain.AccessoryParts.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.AccessoryParts;

public sealed class AccessoryPartConfiguration
    : IEntityTypeConfiguration<AccessoryPart>
{
    public void Configure(EntityTypeBuilder<AccessoryPart> builder)
    {
        builder.ToTable("AccessoryParts");

        builder.Property(x => x.NameAr)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.NameEn)
               .HasMaxLength(200)
               .IsRequired();
    }
}
