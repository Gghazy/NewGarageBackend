using Garage.Domain.Clients.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations;

public class IndividualClientConfiguration : IEntityTypeConfiguration<IndividualClient>
{
    public void Configure(EntityTypeBuilder<IndividualClient> b)
    {
        b.ToTable("IndividualClients");

        b.Property(x => x.NationalId).HasMaxLength(20).IsRequired();
        b.HasIndex(x => x.NationalId).IsUnique();
        b.Property(x => x.BirthDate);

    }
}

