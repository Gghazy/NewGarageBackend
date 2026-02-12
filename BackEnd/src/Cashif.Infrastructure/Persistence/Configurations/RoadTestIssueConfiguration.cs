using Cashif.Domain.AccessoryIssues.Entity;
using Cashif.Domain.ExteriorBodyIssues.Entity;
using Cashif.Domain.InteriorIssues.Entity;
using Cashif.Domain.RoadTestIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Infrastructure.Persistence.Configurations
{
    public sealed class RoadTestIssueConfiguration
    : IEntityTypeConfiguration<RoadTestIssue>
    {
        public void Configure(EntityTypeBuilder<RoadTestIssue> builder)
        {
            builder.ToTable("RoadTestIssues");

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
