using Garage.Application.Abstractions;
using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.Branches.Entities;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InsideAndDecorParts.Entity;
using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.InteriorIssues.Entity;
using Garage.Domain.MechIssues.Entities;
using Garage.Domain.MechIssueTypes.Entity;
using Garage.Domain.RoadTestIssues.Entity;
using Garage.Domain.SensorIssues.Entities;
using Garage.Infrastructure.Auth.Entities;
using Garage.Infrastructure.Persistence.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Garage.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUser) : base(options)
    {
        _currentUser = currentUser;
    }

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<SensorIssue> SensorIssues => Set<SensorIssue>();
    public DbSet<MechIssue> MechIssues => Set<MechIssue>();
    public DbSet<MechIssueType> MechIssueTypes => Set<MechIssueType>();
    public DbSet<InteriorIssue> InteriorIssues => Set<InteriorIssue>();
    public DbSet<InteriorBodyIssue> InteriorBodyIssues => Set<InteriorBodyIssue>();
    public DbSet<ExteriorBodyIssue> ExteriorBodyIssues => Set<ExteriorBodyIssue>();
    public DbSet<AccessoryIssue> AccessoryIssues=> Set<AccessoryIssue>();
    public DbSet<RoadTestIssue> RoadTestIssues=> Set<RoadTestIssue>();
    public DbSet<InsideAndDecorPart> InsideAndDecorParts => Set<InsideAndDecorPart>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new Configurations.BranchConfiguration());
        builder.ApplyConfiguration(new Configurations.SensorIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.MechIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.MechIssueTypeConfiguration());
        builder.ApplyConfiguration(new Configurations.InteriorIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.InteriorBodyIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.ExteriorBodyIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.AccessoryIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.RoadTestIssueConfiguration());
        builder.ApplyConfiguration(new Configurations.InsideAndDecorPartConfiguration());

        builder.Entity<Branch>().HasData(
         new  { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Code = "CEN", NameAr = "الفرع الرئيسي", NameEn = "Central Branch", IsActive = true },
         new  { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Code = "ALX", NameAr = "فرع الإسكندرية", NameEn = "Alexandria Branch", IsActive = true }
     );
    }


    public override Task<int> SaveChangesAsync( CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreatedBy(_currentUser.UserId);

            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdatedBy(_currentUser.UserId);
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}

