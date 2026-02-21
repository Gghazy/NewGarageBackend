using Garage.Application.Abstractions;
using Garage.Domain.Employees.Entities;
using Garage.Infrastructure.Auth.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Garage.Infrastructure.Auth;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = default!;
    public string Audience { get; set; } = default!;
    public string Key { get; set; } = default!;
    public int ExpMinutes { get; set; } = 120;
}

public class JwtTokenService(IOptions<JwtOptions> options) : IJwtTokenService
{
    private readonly JwtOptions _opt = options.Value;

    public (string token, DateTime expiresAt) CreateToken(Guid? userId,string employeeNameAr,string employeeNameEn, string email, IList<Claim>? claims)
    {
        var now = DateTime.UtcNow;

        var identityClaims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

      identityClaims.AddRange(claims ?? Enumerable.Empty<Claim>());

        identityClaims.Add(new Claim("employee_name_ar", employeeNameAr));
        identityClaims.Add(new Claim("employee_name_en", employeeNameEn));
        identityClaims.Add(new Claim(ClaimTypes.Name, employeeNameEn));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = now.AddMinutes(_opt.ExpMinutes);

        var token = new JwtSecurityToken(_opt.Issuer, _opt.Audience, identityClaims, expires: expires, signingCredentials: creds);
        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

}

