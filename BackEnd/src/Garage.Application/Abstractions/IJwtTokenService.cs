using System.Security.Claims;

namespace Garage.Application.Abstractions;
public interface IJwtTokenService
{
    (string token, DateTime expiresAt) CreateToken(Guid? userId, string employeeNameAr, string employeeNameEn, string email, IList<Claim>? claims);
}

