using Garage.Domain.ExteriorBodyParts.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.ExteriorBodyParts;

public sealed class ExteriorBodyPartConfiguration
    : IEntityTypeConfiguration<ExteriorBodyPart>
{
    public void Configure(EntityTypeBuilder<ExteriorBodyPart> builder)
    {
        builder.ToTable("ExteriorBodyParts");

        builder.Property(x => x.NameAr)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.NameEn)
               .HasMaxLength(200)
               .IsRequired();
    }
}
