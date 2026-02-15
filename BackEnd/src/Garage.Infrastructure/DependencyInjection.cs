using Garage.Application.Abstractions;
using Garage.Application.Abstractions.Repositories;
using Garage.Application.Lookup.Queries.GetAllPagination;
using Garage.Contracts.Common;
using Garage.Contracts.Lookup;
using Garage.Infrastructure.Auth;
using Garage.Infrastructure.Auth.Entities;
using Garage.Infrastructure.Authorization;
using Garage.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Garage.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var conn = config.GetConnectionString("Default");
        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlServer(conn));

        services.AddIdentityCore<AppUser>(o => {
            o.User.RequireUniqueEmail = true;
            o.Password.RequireDigit = false;
            o.Password.RequireLowercase = false;
            o.Password.RequireUppercase = false;
            o.Password.RequireNonAlphanumeric = false;
            o.Password.RequiredLength = 6;
            o.Password.RequiredUniqueChars = 0;
        })
            .AddRoles<AppRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IIdentityService, IdentityService>();

        // Authorization handler
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

        // Register permission-based policies (perm:<permission>)
        services.AddAuthorization(options =>
        {
            PermissionPolicies.AddPermissionPolicies(options);
        });

        // Repositories + UoW
        services.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped(typeof(ILookupRepository<>), typeof(LookupRepository<>));
        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();

        return services;
    }
}

