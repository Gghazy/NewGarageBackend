using Garage.Domain.InvoiceManagement.Invoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Invoices;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> b)
    {
        b.ToTable("Invoices");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // -- Type & Status --------------------------------------------------------
        b.Property(x => x.Type).IsRequired();
        b.Property(x => x.Status).IsRequired();

        // -- Invoice Number -------------------------------------------------------
        b.Property(x => x.InvoiceNumber).HasMaxLength(20).IsRequired(false);
        b.HasIndex(x => x.InvoiceNumber).IsUnique().HasFilter("[InvoiceNumber] IS NOT NULL");

        // -- Examination link -----------------------------------------------------
        b.Property(x => x.ExaminationId).IsRequired(false);
        b.HasIndex(x => x.ExaminationId).IsUnique().HasFilter("[ExaminationId] IS NOT NULL");

        // -- Original invoice link (for refunds) ---------------------------------
        b.Property(x => x.OriginalInvoiceId).IsRequired(false);
        b.HasIndex(x => x.OriginalInvoiceId);

        // -- Meta -----------------------------------------------------------------
        b.Property(x => x.Notes).HasMaxLength(1000).IsRequired(false);
        b.Property(x => x.DueDate).IsRequired(false);

        // -- Client reference (owned) ---------------------------------------------
        b.OwnsOne(x => x.Client, c =>
        {
            c.Property(p => p.ClientId)
                .HasColumnName("ClientId")
                .IsRequired();

            c.Property(p => p.NameAr)
                .HasColumnName("ClientNameAr")
                .HasMaxLength(200)
                .IsRequired();

            c.Property(p => p.NameEn)
                .HasColumnName("ClientNameEn")
                .HasMaxLength(200)
                .IsRequired();

            c.Property(p => p.PhoneNumber)
                .HasColumnName("ClientPhone")
                .HasMaxLength(20)
                .IsRequired();

            c.Property(p => p.Email)
                .HasColumnName("ClientEmail")
                .HasMaxLength(200)
                .IsRequired(false);
        });

        // -- Branch reference (owned) ---------------------------------------------
        b.OwnsOne(x => x.Branch, br =>
        {
            br.Property(p => p.BranchId)
                .HasColumnName("BranchId")
                .IsRequired();

            br.Property(p => p.NameAr)
                .HasColumnName("BranchNameAr")
                .HasMaxLength(200)
                .IsRequired();

            br.Property(p => p.NameEn)
                .HasColumnName("BranchNameEn")
                .HasMaxLength(200)
                .IsRequired();
        });

        // -- Financials -----------------------------------------------------------
        b.OwnsOne(x => x.TotalPrice, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("TotalAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("TotalCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.OwnsOne(x => x.DiscountAmount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("DiscountAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("DiscountCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.Property(x => x.TaxRate)
            .HasColumnType("decimal(5,4)")
            .IsRequired()
            .HasDefaultValue(0.15m);

        b.OwnsOne(x => x.TaxAmount, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("TaxAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("TaxCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        b.OwnsOne(x => x.TotalWithTax, m =>
        {
            m.Property(p => p.Amount)
                .HasColumnName("TotalWithTaxAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            m.Property(p => p.Currency)
                .HasColumnName("TotalWithTaxCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // -- Items ----------------------------------------------------------------
        b.HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey("InvoiceId")
            .OnDelete(DeleteBehavior.Cascade);

        // -- Payments -------------------------------------------------------------
        b.HasMany(x => x.Payments)
            .WithOne()
            .HasForeignKey("InvoiceId")
            .OnDelete(DeleteBehavior.Cascade);

        // -- History --------------------------------------------------------------
        b.HasMany(x => x.History)
            .WithOne()
            .HasForeignKey(x => x.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // -- Audit ----------------------------------------------------------------
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");
    }
}
