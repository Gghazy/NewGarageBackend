using Garage.Domain.InteriorBodyParts.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.InteriorBodyParts;

public sealed class InteriorBodyPartConfiguration
    : IEntityTypeConfiguration<InteriorBodyPart>
{
    public void Configure(EntityTypeBuilder<InteriorBodyPart> builder)
    {
        builder.ToTable("InteriorBodyParts");

        builder.Property(x => x.NameAr)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.NameEn)
               .HasMaxLength(200)
               .IsRequired();
    }
}
