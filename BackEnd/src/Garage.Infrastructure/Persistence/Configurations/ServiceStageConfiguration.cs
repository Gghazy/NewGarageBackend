using Garage.Domain.Services.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence.Configurations
{

    public class ServiceStageConfiguration : IEntityTypeConfiguration<ServicesStage>
    {
        public void Configure(EntityTypeBuilder<ServicesStage> b)
        {
            b.ToTable("ServiceStages");

            b.HasKey(x => x.Id);

            b.Property(x => x.StageValue)
                   .IsRequired();

            b.HasIndex(x => new { x.ServiceId, x.StageValue })
                   .IsUnique();


        }
    }
}



