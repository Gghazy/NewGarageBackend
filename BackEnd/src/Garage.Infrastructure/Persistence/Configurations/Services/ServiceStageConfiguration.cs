using Garage.Domain.Services.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Garage.Infrastructure.Persistence.Configurations.Services
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



