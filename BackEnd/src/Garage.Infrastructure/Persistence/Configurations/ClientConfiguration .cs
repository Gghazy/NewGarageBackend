using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {
        b.ToTable("Clients");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id) .ValueGeneratedNever();
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);  
        b.Property(x => x.Type).HasConversion<int>().IsRequired();
        b.HasIndex(x => x.UserId).IsUnique();
        b.Property(x => x.UserId) .IsRequired();

    }
}

