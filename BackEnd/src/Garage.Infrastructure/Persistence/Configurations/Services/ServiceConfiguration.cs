using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Services;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> b)
    {
        b.ToTable("Services");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");


        b.HasMany(x => x.Stages)
             .WithOne(x => x.Service)
             .HasForeignKey(x => x.ServiceId)
             .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.Prices)
           .WithOne()
           .HasForeignKey(x => x.ServiceId)
           .OnDelete(DeleteBehavior.Restrict);


    }
}

