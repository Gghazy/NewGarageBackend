

using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations;
public class ServicePriceConfiguration : IEntityTypeConfiguration<ServicePrice>
{
    public void Configure(EntityTypeBuilder<ServicePrice> b)
    {
        b.ToTable("ServicePrices");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id)
               .ValueGeneratedNever();


        b.HasIndex(x => new { x.ServiceId, x.MarkId, x.FromYear, x.ToYear })
         .IsUnique();


    }
}

