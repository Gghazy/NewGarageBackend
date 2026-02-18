using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.MechIssues.Entities;
using Garage.Domain.Terms.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Terms;

public class TermsConfiguration : IEntityTypeConfiguration<Term>
{
    public void Configure(EntityTypeBuilder<Term> b)
    {
        b.ToTable("Terms");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}

