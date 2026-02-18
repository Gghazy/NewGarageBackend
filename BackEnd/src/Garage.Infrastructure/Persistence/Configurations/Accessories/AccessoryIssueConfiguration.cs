using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InteriorIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence.Configurations.Accessories
{
    public sealed class AccessoryIssueConfiguration
    : IEntityTypeConfiguration<AccessoryIssue>
    {
        public void Configure(EntityTypeBuilder<AccessoryIssue> builder)
        {
            builder.ToTable("AccessoryIssues");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.NameAr)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.NameEn)
                   .HasMaxLength(200)
                   .IsRequired();
        }
    }
}

