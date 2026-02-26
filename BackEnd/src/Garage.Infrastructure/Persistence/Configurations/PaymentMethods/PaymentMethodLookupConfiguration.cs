using Garage.Domain.PaymentMethods.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.PaymentMethods;

public class PaymentMethodLookupConfiguration : IEntityTypeConfiguration<PaymentMethodLookup>
{
    public void Configure(EntityTypeBuilder<PaymentMethodLookup> b)
    {
        b.ToTable("PaymentMethods");

        b.HasKey(x => x.Id);

        b.Property(x => x.Id)
               .ValueGeneratedNever();
        b.Property(x => x.NameAr).IsRequired().HasMaxLength(200);
        b.Property(x => x.NameEn).IsRequired().HasMaxLength(200);
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
