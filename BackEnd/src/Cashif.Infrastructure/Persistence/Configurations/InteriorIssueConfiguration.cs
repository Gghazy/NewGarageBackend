using Cashif.Domain.InteriorIssues.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cashif.Infrastructure.Persistence.Configurations
{
    public sealed class InteriorIssueConfiguration
    : IEntityTypeConfiguration<InteriorIssue>
    {
        public void Configure(EntityTypeBuilder<InteriorIssue> builder)
        {
            builder.ToTable("InteriorIssues");

            builder.Property(x => x.NameAr)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.NameEn)
                   .HasMaxLength(200)
                   .IsRequired();
        }
    }
}
