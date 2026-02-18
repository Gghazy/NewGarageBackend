using Garage.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Clients;

public class IndividualClientConfiguration : IEntityTypeConfiguration<IndividualClient>
{
    public void Configure(EntityTypeBuilder<IndividualClient> b)
    {




        b.OwnsOne(x => x.Address, a =>
        {
            a.Property(p => p.StreetName).HasMaxLength(200).HasColumnName("StreetName");
            a.Property(p => p.AdditionalStreetName).HasMaxLength(200).HasColumnName("AdditionalStreetName");
            a.Property(p => p.CityName).HasMaxLength(200).HasColumnName("CityName");
            a.Property(p => p.PostalZone).HasMaxLength(50).HasColumnName("PostalZone");
            a.Property(p => p.CountrySubentity).HasMaxLength(200).HasColumnName("CountrySubentity");
            a.Property(p => p.CountryCode).HasMaxLength(10).HasColumnName("CountryCode");
            a.Property(p => p.BuildingNumber).HasMaxLength(50).HasColumnName("BuildingNumber");
            a.Property(p => p.CitySubdivisionName).HasMaxLength(200).HasColumnName("CitySubdivisionName");
        });


    }
}

