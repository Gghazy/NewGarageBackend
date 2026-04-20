using Domain.ExaminationManagement.Examinations;
using Garage.Domain.ExaminationManagement.Examinations;
using Garage.Domain.AccessoryIssues.Entity;
using Garage.Domain.AccessoryParts.Entity;
using Garage.Domain.ExaminationManagement.Vehicles;
using Garage.Domain.Branches.Entities;
using Garage.Domain.CarMarkes.Entity;
using Garage.Domain.ClientResources.Entities;
using Garage.Domain.Clients.Entities;
using Garage.Domain.Cranes.Entity;
using Garage.Domain.Employees.Entities;
using Garage.Domain.ExteriorBodyIssues.Entity;
using Garage.Domain.InsideAndDecorParts.Entity;
using Garage.Domain.InsideAndDecorPartIssues.Entity;
using Garage.Domain.InteriorBodyParts.Entity;
using Garage.Domain.ExteriorBodyParts.Entity;
using Garage.Domain.InteriorBodyIssues.Entity;
using Garage.Domain.Manufacturers.Entity;
using Garage.Domain.PaymentMethods.Entity;
using Garage.Domain.MechIssues.Entity;
using Garage.Domain.MechParts.Entities;
using Garage.Domain.MechPartTypes.Entity;
using Garage.Domain.RoadTestIssues.Entity;
using Garage.Domain.SensorIssues.Entities;
using Garage.Domain.ServiceDiscounts.Entities;
using Garage.Domain.ServicePointRules.Entities;
using Garage.Domain.ServicePrices.Entities;
using Garage.Domain.Services.Entities;
using Garage.Domain.InvoiceManagement.Invoices;
using Garage.Domain.Terms.Entity;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Garage.Application.Abstractions
{
    public interface IApplicationDbContext
    {

        DbSet<AppUser> Users { get; }
        DbSet<AppRole> Roles { get; }
        DbSet<IdentityUserRole<Guid>> UserRoles { get; }

        #region Look Up Tables
        DbSet<Branch> Branches { get; }
        DbSet<SensorIssue> SensorIssues { get; }
        DbSet<MechPart> MechParts { get; }
        DbSet<MechPartType> MechPartTypes { get; }
        DbSet<MechIssue> MechIssues { get; }
        DbSet<InteriorBodyIssue> InteriorBodyIssues { get; }
        DbSet<ExteriorBodyIssue> ExteriorBodyIssues { get; }
        DbSet<AccessoryIssue> AccessoryIssues { get; }
        DbSet<AccessoryPart> AccessoryParts { get; }
        DbSet<RoadTestIssue> RoadTestIssues { get; }
        DbSet<RoadTestIssueType> RoadTestIssueTypes { get; }
        DbSet<InsideAndDecorPart> InsideAndDecorParts { get; }
        DbSet<InsideAndDecorPartIssue> InsideAndDecorPartIssues { get; }
        DbSet<InteriorBodyPart> InteriorBodyParts { get; }
        DbSet<ExteriorBodyPart> ExteriorBodyParts { get; }
        DbSet<CarMark> CarMarks { get; }
        DbSet<Manufacturer> Manufacturers { get; }
        DbSet<Crane> Cranes { get; }
        DbSet<Term> Terms { get; }
        DbSet<ClientResource> ClientResources { get; }
        DbSet<PaymentMethodLookup> PaymentMethodLookups { get; }
        #endregion

        #region Services
        DbSet<ServicesStage> ServicesStages { get; }
        DbSet<Service> Services { get; }
        DbSet<ServicePrice> ServicePrices { get; }
        DbSet<ServiceDiscount> ServiceDiscounts { get; }
        DbSet<ServicePointRule> ServicePointRules { get; }
        #endregion

        #region Employees
        DbSet<Employee> Employees { get; }
        #endregion

        #region Clients
        DbSet<Client> Clients { get; }
        DbSet<CompanyClient> CompanyClients { get; }
        DbSet<IndividualClient> IndividualClients { get; }
        #endregion

        #region Examinations
        DbSet<Vehicle>     Vehicles     { get; }
        DbSet<Examination> Examinations { get; }
        DbSet<SensorStageResult> SensorStageResults { get; }
        DbSet<DashboardIndicatorsStageResult> DashboardIndicatorsStageResults { get; }
        DbSet<InteriorDecorStageResult> InteriorDecorStageResults { get; }
        DbSet<InteriorBodyStageResult> InteriorBodyStageResults { get; }
        DbSet<ExteriorBodyStageResult> ExteriorBodyStageResults { get; }
        #endregion

        #region Invoices
        DbSet<Invoice> Invoices { get; }
        #endregion
    }
}
