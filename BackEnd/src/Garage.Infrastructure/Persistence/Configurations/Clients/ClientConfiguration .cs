using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Clients.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Clients;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> b)
    {

        b.ToTable("Clients");
        b.HasKey(x => x.Id);

        b.Property(x => x.UserId).IsRequired();
        b.HasIndex(x => x.UserId).IsUnique();

        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired() .HasMaxLength(200);

        b.Property(x => x.Type)
          .HasConversion(
              v => v.Value,                 
              v => ClientType.FromValue(v)) 
          .HasMaxLength(30)
          .IsRequired();

        b.Property(x => x.Type).IsRequired();

        b.HasIndex(x => x.Type);
        b.HasIndex(x => x.NameAr);
        b.HasIndex(x => x.NameEn);

        b.HasDiscriminator<int>("ClientType")
            .HasValue<CompanyClient>(ClientType.Company.Value)
            .HasValue<IndividualClient>(ClientType.Individual.Value);

    }
}

