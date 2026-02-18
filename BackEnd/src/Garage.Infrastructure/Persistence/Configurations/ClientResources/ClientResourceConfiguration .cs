using Garage.Domain.ClientResources.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Manufacturers;

public class ClientResourceConfiguration : IEntityTypeConfiguration<ClientResource>
{
    public void Configure(EntityTypeBuilder<ClientResource> b)
    {
        b.ToTable("ClientResources");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}

