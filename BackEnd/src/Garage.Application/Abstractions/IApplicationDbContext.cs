using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.Clients.Entities;
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
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Abstractions
{
    public interface IApplicationDbContext
    {

        public DbSet<AppUser> Users { get; }
        public DbSet<AppRole> Roles { get; }
        public DbSet<IdentityUserRole<Guid>> UserRoles { get; }

        #region Look Up Tables
        DbSet<Branch> Branches { get; }
        DbSet<SensorIssue> SensorIssues { get; }
        DbSet<MechIssue> MechIssues { get; }
        DbSet<MechIssueType> MechIssueTypes { get; }
        DbSet<InteriorIssue> InteriorIssues { get; }
        DbSet<InteriorBodyIssue> InteriorBodyIssues { get; }
        DbSet<ExteriorBodyIssue> ExteriorBodyIssues { get; }
        DbSet<AccessoryIssue> AccessoryIssues { get; }
        DbSet<RoadTestIssue> RoadTestIssues { get; }
        DbSet<InsideAndDecorPart> InsideAndDecorParts { get; }
        DbSet<CarMark> CarMarkes { get; }
        DbSet<Manufacturer> Manufacturers { get; }
        DbSet<Crane> Cranes { get; }
        DbSet<Term> Terms { get; }
        #endregion

        #region Services
        DbSet<ServicesStage> ServicesStages { get; }
        DbSet<Service> Services { get; }
        DbSet<ServicePrice> ServicePrices { get; }
        #endregion

        #region Employees
        DbSet<Employee> Employees { get; }
        #endregion

        #region Clients
        DbSet<Client> Clients { get; }
        DbSet<CompanyClient> CompanyClients { get; }
        DbSet<IndividualClient> IndividualClients { get; }
        #endregion
    }
}
