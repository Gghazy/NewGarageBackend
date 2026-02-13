using Garage.Application.Abstractions;
using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMark.Entity;
using Garage.Domain.Common.Primitives;
using Garage.Domain.Cranes.Entity;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InsideAndDecorParts.Entity;
using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.InteriorIssues.Entity;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.MechIssues.Entities;
using Garage.Domain.MechIssueTypes.Entity;
using Garage.Domain.RoadTestIssues.Entity;
using Garage.Domain.SensorIssues.Entities;
using Garage.Domain.Services.Entities;
using Garage.Domain.Terms.Entity;
using Garage.Infrastructure.Auth.Entities;
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
    public DbSet<CarMark> CarMarkes => Set<CarMark>();
    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<ServicesStage> ServicesStages => Set<ServicesStage>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Crane> Cranes => Set<Crane>();
    public DbSet<Term> Terms => Set<Term>();

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
        builder.ApplyConfiguration(new Configurations.CarMarkConfiguration());
        builder.ApplyConfiguration(new Configurations.ManufacturerConfiguration());
        builder.ApplyConfiguration(new Configurations.ServiceConfiguration());
        builder.ApplyConfiguration(new Configurations.ServicePriceConfiguration());
        builder.ApplyConfiguration(new Configurations.ServiceStageConfiguration());
        builder.ApplyConfiguration(new Configurations.CraneConfiguration());
        builder.ApplyConfiguration(new Configurations.TermsConfiguration());

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

