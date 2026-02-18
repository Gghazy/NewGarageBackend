using Garage.Domain.Branches.Entities;
using Garage.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage.Infrastructure.Persistence.Configurations.Employees;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> b)
    {
        b.ToTable("Employees");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("GETUTCDATE()");

        b.Property(x => x.UserId) .IsRequired();

        b.HasIndex(x => x.UserId).IsUnique();

        b.Property(x => x.NameAr).HasMaxLength(200).IsRequired();

        b.Property(x => x.NameEn).HasMaxLength(200).IsRequired();


        b.HasOne<Branch>()
              .WithMany()
              .HasForeignKey(x => x.BranchId)
              .OnDelete(DeleteBehavior.Restrict);
    }
}
