using Garage.Application.Abstractions;
using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Common.Primitives;
using Garage.Domain.Cranes.Entity;
using Garage.Domain.Employees.Entities;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InsideAndDecorParts.Entity;
using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.InteriorIssues.Entity;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.MechIssues.Entities;
using Garage.Domain.MechIssueTypes.Entity;
using Garage.Domain.RoadTestIssues.Entity;
using Garage.Domain.SensorIssues.Entities;
using Garage.Domain.ServicePrices.Entities;
using Garage.Domain.Services.Entities;
using Garage.Domain.Terms.Entity;
using Garage.Domain.Users.Permissions;
using Garage.Infrastructure.Auth.Entities;
using Garage.Infrastructure.Persistence.Configurations.Accessories;
using Garage.Infrastructure.Persistence.Configurations.Branches;
using Garage.Infrastructure.Persistence.Configurations.CarMarks;
using Garage.Infrastructure.Persistence.Configurations.Clients;
using Garage.Infrastructure.Persistence.Configurations.Crans;
using Garage.Infrastructure.Persistence.Configurations.Employees;
using Garage.Infrastructure.Persistence.Configurations.ExteriorBodyIssues;
using Garage.Infrastructure.Persistence.Configurations.InsideAndDecorParts;
using Garage.Infrastructure.Persistence.Configurations.InteriorBodyIssues;
using Garage.Infrastructure.Persistence.Configurations.InteriorIssues;
using Garage.Infrastructure.Persistence.Configurations.Manufacturers;
using Garage.Infrastructure.Persistence.Configurations.MechIssues;
using Garage.Infrastructure.Persistence.Configurations.RoadTestIssues;
using Garage.Infrastructure.Persistence.Configurations.SensorIssues;
using Garage.Infrastructure.Persistence.Configurations.Services;
using Garage.Domain.ClientResources.Entities;
using Garage.Infrastructure.Persistence.Configurations.Terms;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace Garage.Infrastructure.Persistence;

public sealed class ApplicationDbContext
    : IdentityDbContext<AppUser, AppRole, Guid>, IApplicationDbContext
{
    private readonly ICurrentUserService _currentUser;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUser) : base(options)
    {
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
    }

        DbSet<AppUser> IApplicationDbContext.Users => Users;
    DbSet<AppRole> IApplicationDbContext.Roles => Roles;
    DbSet<IdentityUserRole<Guid>> IApplicationDbContext.UserRoles => UserRoles;

    #region Lookups
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<SensorIssue> SensorIssues => Set<SensorIssue>();
    public DbSet<MechIssue> MechIssues => Set<MechIssue>();
    public DbSet<MechIssueType> MechIssueTypes => Set<MechIssueType>();
    public DbSet<InteriorIssue> InteriorIssues => Set<InteriorIssue>();
    public DbSet<InteriorBodyIssue> InteriorBodyIssues => Set<InteriorBodyIssue>();
    public DbSet<ExteriorBodyIssue> ExteriorBodyIssues => Set<ExteriorBodyIssue>();
    public DbSet<AccessoryIssue> AccessoryIssues => Set<AccessoryIssue>();
    public DbSet<RoadTestIssue> RoadTestIssues => Set<RoadTestIssue>();
    public DbSet<InsideAndDecorPart> InsideAndDecorParts => Set<InsideAndDecorPart>();

    // Tip: fix typo CarMarkes -> CarMarks (rename if you can)
    public DbSet<CarMark> CarMarks => Set<CarMark>();

    public DbSet<Manufacturer> Manufacturers => Set<Manufacturer>();
    public DbSet<Crane> Cranes => Set<Crane>();
    public DbSet<Term> Terms => Set<Term>();
    public DbSet<ClientResource> ClientResources => Set<ClientResource>();
    #endregion

    #region Services
    public DbSet<ServicesStage> ServicesStages => Set<ServicesStage>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<ServicePrice> ServicePrices => Set<ServicePrice>();
    #endregion

    #region Employees
    public DbSet<Employee> Employees => Set<Employee>();
    #endregion

    #region Clients
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<CompanyClient> CompanyClients => Set<CompanyClient>();
    public DbSet<IndividualClient> IndividualClients => Set<IndividualClient>();
    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global soft-delete filter: automatically excludes IsDeleted = true from ALL queries
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType) && !entityType.IsAbstract())
            {
                var param = Expression.Parameter(entityType.ClrType, "e");
                var prop = Expression.Property(param, nameof(Entity.IsDeleted));
                var filter = Expression.Lambda(Expression.Not(prop), param);
                entityType.SetQueryFilter(filter);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditInfo()
    {
        var userId = _currentUser.UserId; // Guid? or Guid (based on your service)


        foreach (var entry in ChangeTracker.Entries<Entity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.SetCreatedBy(userId);

            if (entry.State == EntityState.Modified)
                entry.Entity.SetUpdatedBy(userId);
        }
    }
}


