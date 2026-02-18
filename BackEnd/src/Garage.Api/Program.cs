using Garage.Api;
using Garage.Api.Localization;
using Garage.Api.Middleware;
using Garage.Application.Auth.Commands.RegisterUser;
using Garage.Domain.Common.Primitives;
using Garage.Infrastructure;
using Garage.Infrastructure.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------- Services ---------------------------
builder.Services.AddControllers()
               .AddDataAnnotationsLocalization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(ConfigureSwagger);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddLookupHandlersForAllEntities(typeof(LookupBase).Assembly);

builder.Services.AddInfrastructure(builder.Configuration);

// JSON Localization
builder.Services.AddJsonLocalization();

// JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetRequiredSection("Jwt"));
builder.Services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
// AuthN/AuthZ
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(ConfigureJwtBearer(builder.Configuration));

builder.Services.AddAuthorization();
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

// --------------------------- Build ---------------------------
var app = builder.Build();

// --------------------------- Localization ---------------------------
var supportedCultures = new[] { new CultureInfo("ar"), new CultureInfo("en") };
var locOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("ar"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
};
locOptions.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider());
locOptions.RequestCultureProviders.Insert(1, new AcceptLanguageHeaderRequestCultureProvider());
app.UseRequestLocalization(locOptions);

// --------------------------- Pipeline ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

// --------------------------- Seed ---------------------------
using (var scope = app.Services.CreateScope())
{
    await IdentitySeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();

// =========================== Helpers ===========================
static void ConfigureSwagger(SwaggerGenOptions c)
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
    };

    c.AddSecurityDefinition("JwtToken", jwtSecurityScheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
}

static Action<JwtBearerOptions> ConfigureJwtBearer(ConfigurationManager config)
{
    var section = config.GetRequiredSection("Jwt");
    var keyString = section["Key"] ?? throw new InvalidOperationException("Jwt:Key is missing.");
    var keyBytes = Encoding.UTF8.GetBytes(keyString);

    if (keyBytes.Length < 32)
        throw new InvalidOperationException("Jwt:Key must be at least 32 bytes (256 bits) for HS256.");

    var signingKey = new SymmetricSecurityKey(keyBytes);

    return options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false; 
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidIssuer = section["Issuer"],
            ValidAudience = section["Audience"],
            NameClaimType = JwtRegisteredClaimNames.Sub,
            RoleClaimType = ClaimTypes.Role
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(authHeader))
                {
                    if (authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                        context.Token = authHeader.Substring("Bearer ".Length).Trim();
                    else
                        context.Token = authHeader.Trim();
                }
                return Task.CompletedTask;
            }
        };
    };
}

sealed class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Key))
            return ValidateOptionsResult.Fail("Jwt:Key is required.");

        if (Encoding.UTF8.GetByteCount(options.Key) < 32)
            return ValidateOptionsResult.Fail("Jwt:Key must be at least 32 bytes (256 bits) for HS256.");

        if (string.IsNullOrWhiteSpace(options.Issuer))
            return ValidateOptionsResult.Fail("Jwt:Issuer is required.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail("Jwt:Audience is required.");

        return ValidateOptionsResult.Success;
    }
}

