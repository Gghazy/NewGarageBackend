using Garage.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Clients
{
    public sealed class CompanyClientConfiguration : IEntityTypeConfiguration<CompanyClient>
    {
        public void Configure(EntityTypeBuilder<CompanyClient> builder)
        {

            // ============== Identity (CompanyIdentity VO) ==============
            builder.OwnsOne(x => x.Identity, b =>
            {
                b.Property(p => p.CommercialRegister)
                    .HasMaxLength(50)
                    .HasColumnName("CommercialRegister")
                    .IsRequired();

                b.Property(p => p.TaxNumber)
                    .HasMaxLength(30)
                    .HasColumnName("TaxNumber")
                    .IsRequired();
            });

            // ============== Address (Shared Address VO) ==============
            builder.OwnsOne(x => x.Address, a =>
            {
                a.Property(p => p.StreetName)
                    .HasMaxLength(200)
                    .HasColumnName("StreetName")
                    .IsRequired();

                a.Property(p => p.AdditionalStreetName)
                    .HasMaxLength(200)
                    .HasColumnName("AdditionalStreetName")
                    .IsRequired(false);

                a.Property(p => p.CityName)
                    .HasMaxLength(200)
                    .HasColumnName("CityName")
                    .IsRequired();

                a.Property(p => p.PostalZone)
                    .HasMaxLength(50)
                    .HasColumnName("PostalZone")
                    .IsRequired();

                a.Property(p => p.CountrySubentity)
                    .HasMaxLength(200)
                    .HasColumnName("CountrySubentity")
                    .IsRequired(false);

                a.Property(p => p.CountryCode)
                    .HasMaxLength(10)
                    .HasColumnName("CountryCode")
                    .IsRequired();

                a.Property(p => p.BuildingNumber)
                    .HasMaxLength(50)
                    .HasColumnName("BuildingNumber")
                    .IsRequired();

                a.Property(p => p.CitySubdivisionName)
                    .HasMaxLength(200)
                    .HasColumnName("CitySubdivisionName")
                    .IsRequired(false);
            });

            builder.Navigation(x => x.Identity).IsRequired();
            builder.Navigation(x => x.Address).IsRequired();
        }
    }
}
