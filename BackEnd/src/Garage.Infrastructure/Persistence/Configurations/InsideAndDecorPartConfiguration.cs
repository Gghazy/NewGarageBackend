using Garage.Domain.InsideAndDecorParts.Entity;
using Garage.Domain.InteriorIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence.Configurations
{
    public sealed class InsideAndDecorPartConfiguration
    : IEntityTypeConfiguration<InsideAndDecorPart>
    {
        public void Configure(EntityTypeBuilder<InsideAndDecorPart> builder)
        {
            builder.ToTable("InsideAndDecorParts");

            builder.Property(x => x.NameAr)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.NameEn)
                   .HasMaxLength(200)
                   .IsRequired();
        }
    }
}

