using Garage.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence.Configurations
{
    public class CompanyClientConfiguration : IEntityTypeConfiguration<CompanyClient>
    {
        public void Configure(EntityTypeBuilder<CompanyClient> b)
        {
            b.ToTable("CompanyClients");


            b.Property(x => x.CommercialRegister).HasMaxLength(50).IsRequired();
            b.HasIndex(x => x.CommercialRegister).IsUnique();
            b.Property(x => x.TaxNumber).HasMaxLength(50).IsRequired();
            b.Property(x => x.ContactPerson).HasMaxLength(200);
        }
    }
}
