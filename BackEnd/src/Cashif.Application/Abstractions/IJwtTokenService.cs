using System.Security.Claims;

namespace Cashif.Application.Abstractions;
public interface IJwtTokenService
{
    (string token, DateTime expiresAt) CreateToken(Guid? userId, string email,IList<Claim>? claims);
}
