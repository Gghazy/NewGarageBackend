using Microsoft.AspNetCore.Identity;
namespace Cashif.Infrastructure.Auth.Entities;
public class AppUser : IdentityUser<Guid>
{
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }
}
