using Garage.Domain.InsideAndDecorPartIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.InsideAndDecorPartIssues;

public sealed class InsideAndDecorPartIssueConfiguration
    : IEntityTypeConfiguration<InsideAndDecorPartIssue>
{
    public void Configure(EntityTypeBuilder<InsideAndDecorPartIssue> builder)
    {
        builder.ToTable("InsideAndDecorPartIssues");

        builder.Property(x => x.NameAr)
               .HasMaxLength(200)
               .IsRequired();

        builder.Property(x => x.NameEn)
               .HasMaxLength(200)
               .IsRequired();
    }
}
