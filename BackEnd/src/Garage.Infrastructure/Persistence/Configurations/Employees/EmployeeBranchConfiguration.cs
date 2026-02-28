using Garage.Domain.Branches.Entities;
using Garage.Domain.Employees.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Garage.Infrastructure.Persistence.Configurations.Employees;

public sealed class EmployeeBranchConfiguration : IEntityTypeConfiguration<EmployeeBranch>
{
    public void Configure(EntityTypeBuilder<EmployeeBranch> b)
    {
        b.ToTable("EmployeeBranches");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.Property(x => x.EmployeeId).IsRequired();
        b.Property(x => x.BranchId).IsRequired();

        b.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(x => x.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => x.EmployeeId);
        b.HasIndex(x => new { x.EmployeeId, x.BranchId }).IsUnique();
    }
}
